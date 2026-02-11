using DG.Tweening;
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
            => DOVirtual.Float(StartValue, EndValue, Duration,
                alpha => Target.alpha = alpha);
    }
}