using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using R3.Triggers;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace MyUtils
{
    public interface IInputTriggerObservable
    {
        Observable<Unit> OnTriggerObservable { get; }
    }

    public abstract class AbstractInputTrigger : MonoBehaviour, IInputTriggerObservable
    {
        [Header("Trigger Settings")]
        public bool EnableMouseClick = true;
        public bool EnableTouchInput = true;
        public Key[] TriggerKeys = { Key.Enter };
        public float FirstDelaySeconds = 1f;
        public float TriggerThrottleSeconds = 0.5f;
        public AwaitOperation AwaitOperation = AwaitOperation.Drop;

        private readonly Subject<Unit> _onTriggeredSubject = new();
        public Observable<Unit> OnTriggerObservable => _onTriggeredSubject;

        private Mouse _mouse;
        private Keyboard _keyboard;
        private Touchscreen _touchscreen;

        protected virtual async void Start()
        {
            _onTriggeredSubject.AddTo(this);

            // 入力デバイスを取得
            _mouse = Mouse.current;
            _keyboard = Keyboard.current;
            _touchscreen = Touchscreen.current;

            await UniTask.Delay(TimeSpan.FromSeconds(FirstDelaySeconds), DelayType.Realtime,
                cancellationToken: destroyCancellationToken);

            this.UpdateAsObservable()
                .Where(_ => IsPressed())
                .ThrottleFirst(TimeSpan.FromSeconds(TriggerThrottleSeconds),
                    timeProvider: UnityTimeProvider.UpdateRealtime)
                .SubscribeAwait(async (_, ct) =>
                {
                    _onTriggeredSubject.OnNext(Unit.Default);
                    await OnPress(ct);
                }, AwaitOperation)
                .AddTo(this);
        }

        /// <summary>
        /// 入力が押されたか判定
        /// </summary>
        private bool IsPressed()
        {
            // キーボード入力
            if (_keyboard != null && TriggerKeys.Any(k => _keyboard[k].wasPressedThisFrame))
                return true;

            // マウス左クリック
            if (EnableMouseClick && _mouse?.leftButton.wasPressedThisFrame == true)
                return true;

            // タッチ入力
            if (EnableTouchInput && _touchscreen?.primaryTouch.press.wasPressedThisFrame == true)
                return true;

            return false;
        }

        /// <summary>
        /// 押下後に実行される処理（オーバーライド必須）
        /// </summary>
        protected abstract UniTask OnPress(CancellationToken ct);
    }
}