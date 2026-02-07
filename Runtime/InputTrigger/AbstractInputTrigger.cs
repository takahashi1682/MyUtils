using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using R3.Triggers;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;

namespace MyUtils.InputTrigger
{
    public interface IInputTriggerObservable
    {
        Observable<Unit> OnTriggerObservable { get; }
    }

    public abstract class AbstractInputTrigger : MonoBehaviour, IInputTriggerObservable
    {
        [Header("Basic Settings")]
        [SerializeField] private bool _isCurrentSceneOnly = true;
        [SerializeField] private float _firstDelaySeconds = 1f;
        [SerializeField] private float _triggerThrottleSeconds = 0.5f;
        [SerializeField] private AwaitOperation _awaitOperation = AwaitOperation.Drop;

        [Header("Input Settings")]
        [SerializeField] private bool _isEnableMouseClick = true;
        [SerializeField] private bool _isEnableTouchInput = true;
        [SerializeField] private Key[] _triggerKeys = { Key.Enter };
        [SerializeField] private GamepadButton[] _triggerButtons = { GamepadButton.South };

        private readonly Subject<Unit> _pressedSubject = new();
        public Observable<Unit> OnTriggerObservable => _pressedSubject;

        protected virtual async void Start()
        {
            _pressedSubject.AddTo(this);

            await UniTask.Delay(TimeSpan.FromSeconds(_firstDelaySeconds), DelayType.Realtime,
                cancellationToken: destroyCancellationToken);

            this.UpdateAsObservable()
                .Where(_ => IsPressed())
                .ThrottleFirst(TimeSpan.FromSeconds(_triggerThrottleSeconds),
                    timeProvider: UnityTimeProvider.UpdateRealtime)
                .SubscribeAwait(async (_, ct) =>
                {
                    if (_isCurrentSceneOnly &&
                        SceneManager.GetActiveScene().name != gameObject.scene.name)
                        return;

                    _pressedSubject.OnNext(Unit.Default);
                    await OnPressed(ct);
                }, _awaitOperation).AddTo(this);
        }

        /// <summary>
        /// 入力が押されたか判定
        /// </summary>
        private bool IsPressed()
        {
            // キーボード入力
            if (Keyboard.current != null && _triggerKeys.Any(k => Keyboard.current[k].wasPressedThisFrame))
                return true;

            // ゲームパッド入力
            if (Gamepad.current != null && _triggerButtons.Any(b => Gamepad.current[b].wasPressedThisFrame))
                return true;

            // マウス左クリック
            if (_isEnableMouseClick && Mouse.current?.leftButton.wasPressedThisFrame == true)
                return true;

            // タッチ入力
            if (_isEnableTouchInput && Touchscreen.current?.primaryTouch.press.wasPressedThisFrame == true)
                return true;

            return false;
        }

        /// <summary>
        /// 押下後に実行される処理（オーバーライド必須）
        /// </summary>
        protected abstract UniTask OnPressed(CancellationToken ct);
    }
}