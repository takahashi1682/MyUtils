using UnityEngine;

namespace MyUtils.TweenUtils
{
    public class TweenShake : AbstractDoTween<Transform>
    {
        public float Strength = 0.3f; // 揺れの強さ
        public int Vibrato = 10; // 揺れる回数
        public float Randomness = 90f; // 揺れのランダム性

        private Vector3 _originalPos;

        protected override void Start()
        {
            base.Start();
            _originalPos = Target.localPosition;
        }

        protected override Tween CreateTween()
            => Target.DOShakePosition(Duration, Strength, Vibrato, Randomness)
                .OnComplete(() => Target.localPosition = _originalPos);
    }
}