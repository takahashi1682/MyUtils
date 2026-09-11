using MyUtils.Parameter;
using R3;
using R3.Triggers;
using UnityEngine;

namespace MyUtils
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
        void StartTimer();
        void StopTimer();
        void ResetTimer();
    }

    public enum ETimerType
    {
        /// <summary>
        /// カウントダウン
        /// </summary>
        Countdown,
        /// <summary>
        /// カウントアップ
        /// </summary>
        Countup
    }

    /// <summary>
    /// 基本的なカウントダウン機能
    /// </summary>
    public class BasicTimer : AbstractFloatParameter,
        IBasicTimerObservable,
        IBasicTimerHandler
    {
        [field: SerializeField] public SerializableReactiveProperty<bool> IsPlay { get; private set; } = new();
        [field: SerializeField] public ETimerType TimerType { get; private set; }

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
                .Subscribe(_ =>
                {
                    if (TimerType == ETimerType.Countdown)
                    {
                        Sub(Time.deltaTime);
                    }
                    else
                    {
                        Add(Time.deltaTime);
                    }
                })
                .AddTo(this);

            IsEmpty.Where(x => x).Subscribe(_ =>
            {
                IsPlay.Value = false;
                _onFinish.OnNext(Unit.Default);
            }).AddTo(this);
        }

        public void StartTimer()
        {
            IsPlay.Value = true;
            _onStart.OnNext(Unit.Default);
        }

        public void StopTimer()
            => IsPlay.Value = false;

        public void ResetTimer()
            => SetFull();
    }
}