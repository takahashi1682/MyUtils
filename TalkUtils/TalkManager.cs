using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace MyUtils.TalkUtils
{
    /// <summary>
    ///  会話管理
    /// </summary>
    public class TalkManager : AbstractSingletonBehaviour<TalkManager>
    {
        [Header("スキップ可能設定")]
        [SerializeField] protected bool _clickSkip = true;

        [Header("自動スクロール設定")]
        public float OneCharInterval = 0.04f;
        [SerializeField] protected float _nextLineInterval = 0.8f;
        [SerializeField] protected bool _autoEnd = true;

        [Header("表示設定")]
        public Subject<Unit> TalkStart { get; } = new();
        public Subject<LineData> LineStart { get; } = new();
        public Subject<LineData> LineEnd { get; } = new();
        public Subject<Unit> TalkEnd { get; } = new();
        protected ButtonControl LeftMouseButton;
        protected CancellationToken DestroyCancellationToken;

        protected override void Awake()
        {
            TalkStart.AddTo(this);
            LineStart.AddTo(this);
            LineEnd.AddTo(this);
            TalkEnd.AddTo(this);

            DestroyCancellationToken = destroyCancellationToken;
            LeftMouseButton = Mouse.current.leftButton;
            
            base.Awake();
        }

        /// <summary>
        /// 会話を処理する機能
        /// </summary>
        /// <param name="talk"></param>
        public virtual async UniTask TalkAsync(List<LineData> talk)
        {
            TalkStart.OnNext(Unit.Default);

            foreach (var line in talk)
            {
                await LineAsync(line);
                await UniTask.Yield();
            }

            TalkEnd.OnNext(Unit.Default);
        }

        /// <summary>
        ///  1セリフを処理する機能
        /// </summary>
        /// <param name="lineData"></param>
        public virtual async UniTask LineAsync(LineData lineData)
        {
            LineStart.OnNext(lineData);
            if (_autoEnd)
            {
                // 文字数に応じて自動で次のセリフへ
                float delay = lineData.Lines.Length * OneCharInterval + _nextLineInterval;

                // ボイスの長さも考慮
                if (lineData.Voice != null)
                {
                    delay = Mathf.Max(delay, lineData.Voice.length + _nextLineInterval);
                }

                await UniTask.WhenAny(
                    UniTask.WaitUntil(() => _clickSkip && LeftMouseButton.wasPressedThisFrame,
                        cancellationToken: DestroyCancellationToken),
                    UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: DestroyCancellationToken));
            }
            else
            {
                await UniTask.WaitUntil(() => _clickSkip && LeftMouseButton.wasPressedThisFrame,
                    cancellationToken: DestroyCancellationToken);
            }

            LineEnd.OnNext(lineData);
        }
    }
}