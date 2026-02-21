using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace MyUtils.InputTrigger
{
    public interface IInputTriggerObservable
    {
        Observable<Unit> OnTriggerObservable { get; }
    }

    public abstract class AbstractInputTrigger : MonoBehaviour, IInputTriggerObservable
    {
        public bool IsEnabled = true;

        [Header("Basic Settings")]
        public bool CurrentSceneOnly;
        public float FirstDelaySeconds = 1f;
        public float TriggerThrottleSeconds = 0.2f;
        public AwaitOperation AwaitOperation = AwaitOperation.Drop;

        [Header("Input Settings")]
        [SerializeField] private InputActionReference _inputActionReference;
        protected readonly Subject<Unit> _pressedSubject = new();
        public Observable<Unit> OnTriggerObservable => _pressedSubject;

        protected InputAction _inputAction;

        protected virtual void Awake()
        {
            _inputAction = _inputActionReference.action.Clone();
        }

        protected virtual async void Start()
        {
            _pressedSubject.AddTo(this);

            await UniTask.Delay(TimeSpan.FromSeconds(FirstDelaySeconds), DelayType.Realtime,
                cancellationToken: destroyCancellationToken);

            Observable.FromEvent<InputAction.CallbackContext>(
                    h => _inputAction.performed += h,
                    h => _inputAction.performed -= h,
                    destroyCancellationToken
                )
                .Where(_ => IsEnabled)
                .ThrottleFirst(TimeSpan.FromSeconds(TriggerThrottleSeconds),
                    timeProvider: UnityTimeProvider.UpdateRealtime)
                .SubscribeAwait(async (_, ct) =>
                {
                    if (CurrentSceneOnly && SceneManager.GetActiveScene() != gameObject.scene)
                        return;

                    _pressedSubject.OnNext(Unit.Default);
                    await OnPressed(ct);
                }, AwaitOperation)
                .AddTo(this);
        }

        protected virtual void OnEnable() => _inputAction?.Enable();
        protected virtual void OnDisable() => _inputAction?.Disable();

        /// <summary>
        /// 押下後に実行される処理（オーバーライド必須）
        /// </summary>
        protected abstract UniTask OnPressed(CancellationToken ct);
    }
}