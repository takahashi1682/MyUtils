using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
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

        [Header("Common Settings")]
        public bool CurrentSceneOnly = true;
        public float FirstDelaySeconds = 1f;
        public float TriggerThrottleSeconds = 0.2f;
        public AwaitOperation AwaitOperation = AwaitOperation.Drop;

        private readonly Subject<Unit> _triggerSubject = new();
        public Observable<Unit> OnTriggerObservable => _triggerSubject;

        protected virtual void Start()
        {
            _triggerSubject.AddTo(this);
            InitializeAsync().Forget();
        }

        private async UniTaskVoid InitializeAsync()
        {
            // 初期ディレイ
            await UniTask.Delay(TimeSpan.FromSeconds(FirstDelaySeconds), DelayType.Realtime, cancellationToken: destroyCancellationToken);

            // 各サブクラスで定義された入力ストリームを購読
            CreateInputObservable()
                .Where(_ => IsEnabled)
                .Where(_ => !CurrentSceneOnly || SceneManager.GetActiveScene() == gameObject.scene)
                .ThrottleFirst(TimeSpan.FromSeconds(TriggerThrottleSeconds), UnityTimeProvider.UpdateRealtime)
                .SubscribeAwait(async (_, ct) =>
                {
                    _triggerSubject.OnNext(Unit.Default);
                    await OnPressed(ct);
                }, AwaitOperation)
                .AddTo(this);
        }

        /// <summary>
        /// 入力の発生源を定義する（サブクラスで実装）
        /// </summary>
        protected abstract Observable<Unit> CreateInputObservable();

        protected abstract UniTask OnPressed(CancellationToken ct);
    }
}