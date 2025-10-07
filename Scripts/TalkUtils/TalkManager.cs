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
        [SerializeField] private bool _clickSkip = true;

        [Header("自動スクロール設定")]
        public float OneCharInterval = 0.1f;
        [SerializeField] private float _nextLineInterval = 0.8f;
        [SerializeField] private bool _autoEnd = true;

        [Header("表示設定")]
        public Subject<LineData> LineStart { get; } = new();
        public Subject<LineData> LineEnd { get; } = new();
        private ButtonControl _leftMouseButton;
        private CancellationToken _destroyCancellationToken;

        protected override void Awake()
        {
            base.Awake();
            LineStart.AddTo(this);
            LineEnd.AddTo(this);
            _destroyCancellationToken = destroyCancellationToken;
            _leftMouseButton = Mouse.current.leftButton;
        }

        /// <summary>
        /// 会話を処理する機能
        /// </summary>
        /// <param name="talk"></param>
        public async UniTask TalkAsync(List<LineData> talk)
        {
            foreach (var line in talk)
            {
                await LineAsync(line);
                await UniTask.Yield();
            }
        }

        /// <summary>
        ///  1セリフを処理する機能
        /// </summary>
        /// <param name="lineData"></param>
        public async UniTask LineAsync(LineData lineData)
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
                    UniTask.WaitUntil(() => _clickSkip && _leftMouseButton.wasPressedThisFrame,
                        cancellationToken: _destroyCancellationToken),
                    UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: _destroyCancellationToken));
            }
            else
            {
                await UniTask.WaitUntil(() => _clickSkip && _leftMouseButton.wasPressedThisFrame,
                    cancellationToken: _destroyCancellationToken);
            }

            LineEnd.OnNext(lineData);
        }
    }
}