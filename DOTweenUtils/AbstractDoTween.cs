using DG.Tweening;
using UnityEngine;

namespace MyUtils.DOTweenUtils
{
    public abstract class AbstractDoTween<T> : AbstractTargetBehaviour<T> where T : Component
    {
        public float Duration = 1;
        public float Delay;
        public Ease Ease = Ease.InSine;
        public int Loops = -1;
        public LoopType LoopType = LoopType.Yoyo;
        public LinkBehaviour LinkBehaviour = LinkBehaviour.PauseOnDisablePlayOnEnable;
        public bool IsAutoKill = true;

        public Tween TweenInstance { get; protected set; }
        protected abstract Tween CreateTween();

        protected virtual void Start() => Reload();

        protected virtual void OnDestroy()
            => TweenInstance?.Kill();

        public void Reload()
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
                .SetAutoKill(IsAutoKill);
    }
}