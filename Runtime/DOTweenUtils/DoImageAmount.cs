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
            => Tween.Value(StartValue, EndValue, Duration,
                alpha => Target.fillAmount = alpha);
    }
}