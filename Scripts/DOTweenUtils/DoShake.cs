using DG.Tweening;
using UnityEngine;

namespace MyUtils.DOTweenUtils
{
    public class DoShake : AbstractDoTween
    {
        public float Strength = 0.3f; // 揺れの強さ
        public int Vibrato = 10; // 揺れる回数
        public float Randomness = 90f; // 揺れのランダム性

        private Vector3 _originalPos;

        private void Awake()
        {
            _originalPos = transform.localPosition;
        }

        protected override Tween CreateTween()
            => transform.DOShakePosition(Duration, Strength, Vibrato, Randomness)
                .OnComplete(() => transform.localPosition = _originalPos);
    }
}