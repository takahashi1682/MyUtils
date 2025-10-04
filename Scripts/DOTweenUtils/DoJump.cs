using DG.Tweening;

namespace TUtils.DOTweenUtils
{
    public class DoJump : AbstractDoTween
    {
        public float JumpPower = 2f; // ジャンプの高さ
        public int NumJumps = 1; // ジャンプの回数

        protected override Tween CreateTween()
            => transform.DOLocalJump(transform.localPosition, JumpPower, NumJumps, Duration);
    }
}