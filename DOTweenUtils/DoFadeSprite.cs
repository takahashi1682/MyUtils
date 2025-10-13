using DG.Tweening;
using UnityEngine;

namespace MyUtils.DOTweenUtils
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class DoFadeSprite : AbstractDoTween<SpriteRenderer>
    {
        [Header("DoFadeSprite")] [SerializeField]
        public float StartValue = 1;
        public float EndValue;

        protected override Tween CreateTween()
        {
            var sequence = DOTween.Sequence();
            sequence.Append(Target.material.DOFade(StartValue, 0));
            sequence.Append(Target.material.DOFade(EndValue, Duration));
            return sequence;
        }
    }
}