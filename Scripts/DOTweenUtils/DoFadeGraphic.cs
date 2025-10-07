using DG.Tweening;
using UnityEngine;

namespace MyUtils.DOTweenUtils
{
    [RequireComponent(typeof(CanvasGroup))]
    public class DoFadeGraphic : AbstractDoTween
    {
        [Header("DoFadeGraphic")]
        public float StartValue = 1;
        public float EndValue;

        private CanvasGroup _canvasGroup;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        protected override Tween CreateTween()
            => DOVirtual.Float(StartValue, EndValue, Duration,
                alpha => _canvasGroup.alpha = alpha);
    }
}