using DG.Tweening;
using UnityEngine;

namespace MyUtils.DOTweenUtils
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class DoFadeSprite : AbstractDoTween
    {
        [Header("DoFadeSprite")] [SerializeField]
        public float StartValue = 1;
        public float EndValue;

        private SpriteRenderer _targetSprite;

        private void Awake()
        {
            _targetSprite = GetComponent<SpriteRenderer>();
        }

        protected override Tween CreateTween()
        {
            var sequence = DOTween.Sequence();
            sequence.Append(_targetSprite.material.DOFade(StartValue, 0));
            sequence.Append(_targetSprite.material.DOFade(EndValue, Duration));
            return sequence;
        }
    }
}