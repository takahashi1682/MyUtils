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
    /// 会話の呼び出しは <see cref="Talk(string)"/> で行い、重なった場合の動作は <see cref="ETalkOverlapMode"/> で選ぶ。
    /// </summary>
    public class TalkManager : AbstractActionInputTrigger
    {
        [SerializeField] protected TextAsset _defaultTalkCsv;

        [Header("会話の呼び出しが重なったときの動作")]
        [SerializeField] protected ETalkOverlapMode _overlapMode = ETalkOverlapMode.Drop;

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

        /// <summary>会話中かどうか(順番待ちの会話は含まない)</summary>
        public bool IsTalking => _activeTalkCount > 0;

        // LoadCsvで読み込んだ会話データ
        private TalkData _talkData;

        // 会話の呼び出し。SubscribeAwait で重なったときの動作を制御する
        private readonly Subject<IReadOnlyList<TalkLine>> _talkRequests = new();

        // セリフ送りの入力があったときに完了する(セリフ待機中のみ有効)
        private UniTaskCompletionSource _skipSignal;

        // 読み込み済みのボイス(アドレスごと。解放はしない)
        private readonly Dictionary<string, AudioClip> _voiceCache = new();

        private int _activeTalkCount;

        protected override void Awake()
        {
            base.Awake();

            OnTalkStart.AddTo(this);
            OnLineStart.AddTo(this);
            OnLineEnd.AddTo(this);
            OnTalkEnd.AddTo(this);
            _talkRequests.AddTo(this);

            // 重なった呼び出しの扱いはここで決まる(実行中に変更しても反映されない)
            _talkRequests
                .SubscribeAwait(async (talk, ct) =>
                {
                    try
                    {
                        await RunTalkAsync(talk, ct);
                    }
                    catch (OperationCanceledException)
                    {
                        // Switchによる中断やオブジェクト破棄。正常な終了として扱う
                    }
                }, ToAwaitOperation(_overlapMode))
                .AddTo(this);

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
        /// 会話(複数のセリフ)を開始する。会話中に呼ばれた場合は <see cref="ETalkOverlapMode"/> に従う
        /// </summary>
        public void Talk(IReadOnlyList<TalkLine> talk)
        {
            _talkRequests.OnNext(talk);
        }

        /// <summary>
        /// LoadCsvで読み込んだ会話のうち、指定したキーの会話を開始する。キーがなければ警告を出して何もしない
        /// </summary>
        public void Talk(string key)
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

            Talk(talk);
        }

        /// <summary>
        /// 会話を最初から最後まで進める
        /// </summary>
        protected virtual async UniTask RunTalkAsync(IReadOnlyList<TalkLine> talk, CancellationToken ct)
        {
            _activeTalkCount++;
            try
            {
                using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct, destroyCancellationToken);

                // ボイスは会話の開始前にまとめて読み込む
                await LoadVoicesAsync(talk, cts.Token);

                OnTalkStart.OnNext(Unit.Default);
                try
                {
                    foreach (var line in talk)
                    {
                        await LineAsync(line, cts.Token);

                        // 同じ入力で次のセリフまで飛ばないように1フレーム待つ
                        await UniTask.Yield(cts.Token);
                    }
                }
                finally
                {
                    // 中断されたときも終了を通知する(破棄中は通知しない)
                    if (!destroyCancellationToken.IsCancellationRequested)
                    {
                        OnTalkEnd.OnNext(Unit.Default);
                    }
                }
            }
            finally
            {
                _activeTalkCount--;
            }
        }

        /// <summary>
        /// 1セリフを進める。セリフ送りの入力または自動送りで終了する
        /// </summary>
        public virtual async UniTask LineAsync(TalkLine talkLine, CancellationToken ct = default)
        {
            OnLineStart.OnNext(talkLine);

            // 待ち終わった側の待機を止めるため、リンクしたトークンを使う
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct, destroyCancellationToken);
            var signal = new UniTaskCompletionSource();
            _skipSignal = signal;

            try
            {
                var skip = signal.Task.AttachExternalCancellation(cts.Token);

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
            }
            finally
            {
                cts.Cancel();

                // Switchで次のセリフが先に始まっていたら、そちらの待機は消さない
                if (_skipSignal == signal)
                {
                    _skipSignal = null;
                }

                if (!destroyCancellationToken.IsCancellationRequested)
                {
                    OnLineEnd.OnNext(talkLine);
                }
            }
        }

        /// <summary>
        /// ボイスのアドレスが設定されたセリフのボイスを Addressables から読み込む
        /// </summary>
        protected virtual async UniTask LoadVoicesAsync(IReadOnlyList<TalkLine> talk, CancellationToken ct)
        {
            foreach (var line in talk)
            {
                if (line.Voice != null || string.IsNullOrEmpty(line.VoiceAddress)) continue;

                line.Voice = await LoadVoiceAsync(line.VoiceAddress, ct);
            }
        }

        /// <summary>
        /// アドレスのボイスを読み込む。同じアドレスは再読み込みしない。見つからなければ警告を出してnullを返す
        /// </summary>
        protected virtual async UniTask<AudioClip> LoadVoiceAsync(string address, CancellationToken ct)
        {
            if (_voiceCache.TryGetValue(address, out var cached)) return cached;

            // 存在しないアドレスは例外にせず、ボイスなしとして扱う
            if (!await ExistsAddressAsync(address, ct))
            {
                Debug.LogWarning($"[TalkManager] ボイス '{address}' が見つかりません");
                return null;
            }

            var handle = Addressables.LoadAssetAsync<AudioClip>(address);
            var clip = await handle.Task.AsUniTask().AttachExternalCancellation(ct);

            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogWarning($"[TalkManager] ボイス '{address}' の読み込みに失敗しました");
                return null;
            }

            _voiceCache[address] = clip;
            return clip;
        }

        private static async UniTask<bool> ExistsAddressAsync(string address, CancellationToken ct)
        {
            var handle = Addressables.LoadResourceLocationsAsync(address, typeof(AudioClip));
            try
            {
                var locations = await handle.Task.AsUniTask().AttachExternalCancellation(ct);
                return locations.Count > 0;
            }
            finally
            {
                Addressables.Release(handle);
            }
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

        private static AwaitOperation ToAwaitOperation(ETalkOverlapMode mode)
        {
            return mode switch
            {
                ETalkOverlapMode.Sequential => AwaitOperation.Sequential,
                ETalkOverlapMode.Switch => AwaitOperation.Switch,
                _ => AwaitOperation.Drop
            };
        }
    }
}
