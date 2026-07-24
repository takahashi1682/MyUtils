using UnityEngine;

namespace MyUtils.TweenUtils
{
    [RequireComponent(typeof(CanvasGroup))]
    public class TweenFadeGraphic : AbstractDoTween<CanvasGroup>
    {
        [Header("TweenFadeGraphic")]
        public float StartValue = 1f;
        public float EndValue;

        protected override Tween CreateTween()
            => Tween.Value(StartValue, EndValue, Duration,
                alpha => Target.alpha = alpha);
    }
}