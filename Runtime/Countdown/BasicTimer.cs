using MyUtils.Parameter;
using R3;
using R3.Triggers;
using UnityEngine;

namespace MyUtils.Countdown
{
    /// <summary>
    /// 購読可能なカウントダウン機能
    /// </summary>
    public interface IBasicTimerObservable : IFloatParameter
    {
        Observable<Unit> OnStart { get; }
        Observable<Unit> OnFinish { get; }
    }

    /// <summary>
    /// カウントダウン機能の操作
    /// </summary>
    public interface IBasicTimerHandler
    {
        SerializableReactiveProperty<bool> IsPlay { get; }
        void StartCountdown();
        void StopCountdown();
        void ResetCountdown();
    }

    /// <summary>
    /// 基本的なカウントダウン機能
    /// </summary>
    public class BasicTimer : AbstractFloatParameter,
        IBasicTimerObservable,
        IBasicTimerHandler
    {
        [field: SerializeField] public SerializableReactiveProperty<bool> IsPlay { get; private set; } = new();

        private readonly Subject<Unit> _onStart = new();
        public Observable<Unit> OnStart => _onStart;

        private readonly Subject<Unit> _onFinish = new();
        public Observable<Unit> OnFinish => _onFinish;

        private void Start()
        {
            _onStart.AddTo(this);
            _onFinish.AddTo(this);

            this.UpdateAsObservable()
                .Where(_ => IsPlay.CurrentValue)
                .Subscribe(_ => Sub(Time.deltaTime))
                .AddTo(this);

            IsEmpty.Where(x => x).Subscribe(_ =>
            {
                IsPlay.Value = false;
                _onFinish.OnNext(Unit.Default);
            }).AddTo(this);
        }

        public void StartCountdown()
        {
            IsPlay.Value = true;
            _onStart.OnNext(Unit.Default);
        }

        public void StopCountdown()
            => IsPlay.Value = false;

        public void ResetCountdown()
            => SetFull();
    }
}