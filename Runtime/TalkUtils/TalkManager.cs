using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using MyUtils.Csv;
using MyUtils.InputTrigger;
using R3;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace MyUtils.TalkUtils
{
    /// <summary>
    /// 会話の進行管理。表示や音声は購読側(<see cref="TalkLineViewer"/> / <see cref="TalkLineVoice"/>)が担当する。
    /// セリフ送りの入力は InputActionReference で設定する(未設定なら左クリック)。
    /// </summary>
    public class TalkManager : AbstractActionInputTrigger
    {
        [SerializeField] protected TextAsset _defaultTalkCsv;

        [Header("スキップ設定")]
        [SerializeField] protected bool _clickSkip = true;

        [Header("自動送り設定")]
        public float OneCharInterval = 0.04f;
        [SerializeField] protected float _nextLineInterval = 0.8f;
        [SerializeField] protected bool _autoEnd = true;

        public Subject<Unit> OnTalkStart { get; } = new();
        public Subject<TalkLine> OnLineStart { get; } = new();
        public Subject<TalkLine> OnLineEnd { get; } = new();
        public Subject<Unit> OnTalkEnd { get; } = new();

        // LoadCsvで読み込んだ会話データ
        private TalkData _talkData;

        // セリフ送りの入力があったときに完了する(セリフ待機中のみ有効)
        private UniTaskCompletionSource _skipSignal;

        // 読み込み済みのボイス(アドレスごと。解放はしない)
        private readonly Dictionary<string, AudioClip> _voiceCache = new();

        protected override void Awake()
        {
            base.Awake();

            OnTalkStart.AddTo(this);
            OnLineStart.AddTo(this);
            OnLineEnd.AddTo(this);
            OnTalkEnd.AddTo(this);

            if (_defaultTalkCsv != null)
            {
                LoadCsv(_defaultTalkCsv);
            }
        }

        /// <summary>
        /// CSVを読み込み、キーごとの会話データとして保持する(以前の内容は置き換わる)。
        /// 1行目はヘッダとして読み飛ばし、2行目からをセリフとして扱う
        /// </summary>
        public void LoadCsv(TextAsset textAsset)
        {
            const int headerLineCount = 1;
            _talkData = new TalkData(CsvUtils<TalkLineCsv>.Parse(textAsset, linesToSkip: headerLineCount));
        }

        /// <summary>
        /// 会話(複数のセリフ)を最初から最後まで進める
        /// </summary>
        public virtual async UniTask TalkAsync(IReadOnlyList<TalkLine> talk)
        {
            // ボイスは会話の開始前にまとめて読み込む
            await LoadVoicesAsync(talk);

            OnTalkStart.OnNext(Unit.Default);

            foreach (var line in talk)
            {
                await LineAsync(line);

                // 同じ入力で次のセリフまで飛ばないように1フレーム待つ
                await UniTask.Yield();
            }

            OnTalkEnd.OnNext(Unit.Default);
        }

        /// <summary>
        /// ボイスのアドレスが設定されたセリフのボイスを Addressables から読み込む
        /// </summary>
        protected virtual async UniTask LoadVoicesAsync(IReadOnlyList<TalkLine> talk)
        {
            foreach (var line in talk)
            {
                if (line.Voice != null || string.IsNullOrEmpty(line.VoiceAddress)) continue;

                line.Voice = await LoadVoiceAsync(line.VoiceAddress);
            }
        }

        /// <summary>
        /// アドレスのボイスを読み込む。同じアドレスは再読み込みしない。見つからなければ警告を出してnullを返す
        /// </summary>
        protected virtual async UniTask<AudioClip> LoadVoiceAsync(string address)
        {
            if (_voiceCache.TryGetValue(address, out var cached)) return cached;

            // 存在しないアドレスは例外にせず、ボイスなしとして扱う
            if (!await ExistsAddressAsync(address))
            {
                Debug.LogWarning($"[TalkManager] ボイス '{address}' が見つかりません");
                return null;
            }

            var handle = Addressables.LoadAssetAsync<AudioClip>(address);
            var clip = await handle.Task.AsUniTask().AttachExternalCancellation(destroyCancellationToken);

            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogWarning($"[TalkManager] ボイス '{address}' の読み込みに失敗しました");
                return null;
            }

            _voiceCache[address] = clip;
            return clip;
        }

        private async UniTask<bool> ExistsAddressAsync(string address)
        {
            var handle = Addressables.LoadResourceLocationsAsync(address, typeof(AudioClip));
            try
            {
                var locations = await handle.Task.AsUniTask().AttachExternalCancellation(destroyCancellationToken);
                return locations.Count > 0;
            }
            finally
            {
                Addressables.Release(handle);
            }
        }

        /// <summary>
        /// LoadCsvで読み込んだ会話のうち、指定したキーの会話を進める。キーがなければ警告を出して何もしない
        /// </summary>
        public virtual async UniTask TalkAsync(string key)
        {
            if (_talkData == null)
            {
                Debug.LogWarning("会話データが未読み込みです。先にLoadCsvを呼んでください");
                return;
            }

            if (!_talkData.TryGetValue(key, out var talk))
            {
                Debug.LogWarning($"TalkDataに {key} の会話データがありません");
                return;
            }

            await TalkAsync(talk);
        }

        /// <summary>
        /// 1セリフを進める。セリフ送りの入力または自動送りで終了する
        /// </summary>
        public virtual async UniTask LineAsync(TalkLine talkLine)
        {
            OnLineStart.OnNext(talkLine);

            // 待ち終わった側の待機を止めるため、リンクしたトークンを使う
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken);
            _skipSignal = new UniTaskCompletionSource();
            var skip = _skipSignal.Task.AttachExternalCancellation(cts.Token);

            if (_autoEnd)
            {
                var autoEnd = UniTask.Delay(TimeSpan.FromSeconds(GetAutoEndDelay(talkLine)),
                    cancellationToken: cts.Token);
                await UniTask.WhenAny(skip, autoEnd);
            }
            else
            {
                await skip;
            }

            cts.Cancel();
            _skipSignal = null;
            OnLineEnd.OnNext(talkLine);
        }

        /// <summary>
        /// セリフ送りの入力元。InputActionReference未設定なら左クリックを使う
        /// </summary>
        protected override Observable<Unit> CreateInputObservable()
        {
            if (HasInputAction) return base.CreateInputObservable();

            return Observable.EveryUpdate(destroyCancellationToken)
                .Where(_ => Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
                .Select(_ => Unit.Default);
        }

        /// <summary>
        /// セリフ送りの入力があったとき、待機中のセリフを終わらせる
        /// </summary>
        protected override UniTask OnPressed(CancellationToken ct)
        {
            if (_clickSkip)
            {
                _skipSignal?.TrySetResult();
            }

            return UniTask.CompletedTask;
        }

        // 文字数に応じた自動送りまでの時間(ボイスがあればその長さも考慮する)
        private float GetAutoEndDelay(TalkLine talkLine)
        {
            float delay = talkLine.Text.Length * OneCharInterval + _nextLineInterval;

            if (talkLine.Voice != null)
            {
                delay = Mathf.Max(delay, talkLine.Voice.length + _nextLineInterval);
            }

            return delay;
        }
    }
}