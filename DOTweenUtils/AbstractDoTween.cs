using DG.Tweening;
using UnityEngine;

namespace MyUtils.DOTweenUtils
{
    public interface IDoTween
    {
        public Tween TweenInstance { get; }
        public void Reset();
        public void Play();
        public void PlayBackwards();
        public void Pause();
        public void Restart();
        public void Rewind();
        public void Complete();
        public void Flip();
    }

    public abstract class AbstractDoTween<T> : AbstractTargetBehaviour<T>, IDoTween where T : Component
    {
        public float Duration = 1;
        public float Delay;
        public Ease Ease = Ease.InSine;
        public int Loops = -1;
        public LoopType LoopType = LoopType.Yoyo;
        public LinkBehaviour LinkBehaviour = LinkBehaviour.PauseOnDisable;
        public bool IsAutoPlayOnStart = true;
        public bool IsAutoKill = true;

        public Tween TweenInstance { get; protected set; }

        protected abstract Tween CreateTween();

        protected override void Start()
        {
            base.Start();
            InitializeTween();

            if (IsAutoPlayOnStart)
                Play();
        }

        private void InitializeTween()
        {
            TweenInstance?.Kill();
            TweenInstance = CreateTween();
            ApplyDefaultSettings();
        }

        private void ApplyDefaultSettings()
            => TweenInstance?
                .SetDelay(Delay)
                .SetEase(Ease)
                .SetLoops(Loops, LoopType)
                .SetLink(gameObject, LinkBehaviour)
                .SetAutoKill(IsAutoKill)
                .Pause();

        public void Reset()
        {
            InitializeTween();
            Pause();
            Rewind();
        }

        /// <summary>
        /// Tweenアニメーションを最初から再生または一時停止状態から再開します。
        /// </summary>
        public void Play()
            => TweenInstance?.Play();

        /// <summary>
        /// Tweenアニメーションを逆方向に再生します。
        /// </summary>
        public void PlayBackwards()
            => TweenInstance?.PlayBackwards();

        /// <summary>
        /// Tweenアニメーションを一時停止します。
        /// </summary>
        public void Pause()
            => TweenInstance?.Pause();

        /// <summary>
        /// Tweenを最初から再生し直します。
        /// </summary>
        public void Restart()
            => TweenInstance?.Restart();

        /// <summary>
        /// Tweenを巻き戻し、開始時の状態に戻しますが、再生はしません（一時停止状態になります）。
        /// </summary>
        public void Rewind()
            => TweenInstance?.Rewind();

        /// <summary>
        /// Tweenを即座に完了させ、最終状態へジャンプさせます。
        /// </summary>
        public void Complete()
            => TweenInstance?.Complete();

        /// <summary>
        /// Tweenの進行方向を反転させます（現在再生中であれば逆方向に、一時停止中であれば方向を反転）。
        /// </summary>
        public void Flip()
            => TweenInstance?.Flip();
    }
}