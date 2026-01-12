using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace MyUtils.DOTweenUtils
{
    [RequireComponent(typeof(Image))]
    public class DoImageAmount : AbstractDoTween<Image>
    {
        [Header("DoFadeGraphic")]
        public float StartValue = 1;
        public float EndValue;

        protected override Tween CreateTween()
            => DOVirtual.Float(StartValue, EndValue, Duration,
                alpha => Target.fillAmount = alpha);
    }
}