using R3;
using R3.Triggers;
using TUtils.Parameter;
using UnityEngine;

namespace TUtils.Countdown
{
    /// <summary>
    /// 購読可能なカウントダウン機能
    /// </summary>
    public interface IBasicTimerObservable : IParameter
    {
        ReadOnlyReactiveProperty<bool> IsPlay { get; }
        Observable<Unit> OnStart { get; }
        Observable<Unit> OnFinish { get; }
    }

    /// <summary>
    /// カウントダウン機能の操作
    /// </summary>
    public interface IBasicTimerHandler
    {
        void StartCountdown();
        void StopCountdown();
        void ResetCountdown();
    }

    /// <summary>
    /// 基本的なカウントダウン機能
    /// </summary>
    public class BasicTimer : AbstractParameter,
        IBasicTimerObservable,
        IBasicTimerHandler
    {
        [SerializeField] private SerializableReactiveProperty<bool> _isPlay = new();
        public ReadOnlyReactiveProperty<bool> IsPlay => _isPlay;

        private readonly Subject<Unit> _onStart = new();
        public Observable<Unit> OnStart => _onStart;

        private readonly Subject<Unit> _onFinish = new();
        public Observable<Unit> OnFinish => _onFinish;

        private void Start()
        {
            _onStart.AddTo(this);
            _onFinish.AddTo(this);

            this.UpdateAsObservable()
                .Where(_ => _isPlay.CurrentValue)
                .Subscribe(_ => Sub(Time.deltaTime))
                .AddTo(this);

            IsEmpty.Where(x => x).Subscribe(_ =>
            {
                _isPlay.Value = false;
                _onFinish.OnNext(Unit.Default);
            }).AddTo(this);
        }

        public void StartCountdown()
        {
            _isPlay.Value = true;
            _onStart.OnNext(Unit.Default);
        }

        public void StopCountdown()
            => _isPlay.Value = false;

        public void ResetCountdown()
            => SetFull();
    }
}