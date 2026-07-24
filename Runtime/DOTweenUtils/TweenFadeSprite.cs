using UnityEngine;

namespace MyUtils.DOTweenUtils
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class TweenFadeSprite : AbstractDoTween<SpriteRenderer>
    {
        [Header("TweenFadeSprite")] [SerializeField]
        public float StartValue = 1;
        public float EndValue;

        protected override Tween CreateTween()
        {
            Color color = Target.material.color;
            color.a = StartValue;
            Target.material.color = color;

            return Target.material.DOFade(EndValue, Duration);
        }
    }
}