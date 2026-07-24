using UnityEngine;
using UnityEngine.UI;

namespace MyUtils.TweenUtils
{
    [RequireComponent(typeof(Image))]
    public class TweenImageAmount : AbstractDoTween<Image>
    {
        [Header("TweenImageAmount")]
        public float StartValue = 1;
        public float EndValue;

        protected override Tween CreateTween()
            => Tween.Value(StartValue, EndValue, Duration,
                alpha => Target.fillAmount = alpha);
    }
}