using DG.Tweening;
using UnityEngine;

namespace MyUtils.DOTweenUtils
{
    public class DoShake : AbstractDoTween<Transform>
    {
        public float Strength = 0.3f; // 揺れの強さ
        public int Vibrato = 10; // 揺れる回数
        public float Randomness = 90f; // 揺れのランダム性

        private Vector3 _originalPos;

        protected override void Awake()
        {
            base.Awake();
            _originalPos = _target.localPosition;
        }

        protected override Tween CreateTween()
            => _target.DOShakePosition(Duration, Strength, Vibrato, Randomness)
                .OnComplete(() => _target.localPosition = _originalPos);
    }
}