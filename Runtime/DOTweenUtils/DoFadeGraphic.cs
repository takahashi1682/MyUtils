using UnityEngine;

namespace MyUtils.DOTweenUtils
{
    [RequireComponent(typeof(CanvasGroup))]
    public class DoFadeGraphic : AbstractDoTween<CanvasGroup>
    {
        [Header("DoFadeGraphic")]
        public float StartValue = 1f;
        public float EndValue;

        protected override Tween CreateTween()
            => Tween.Value(StartValue, EndValue, Duration,
                alpha => Target.alpha = alpha);
    }
}