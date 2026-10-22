using UnityEngine;

namespace MyUtils.TweenUtils
{
    [RequireComponent(typeof(Transform))]
    public class TweenJump : AbstractTween<Transform>
    {
        public float JumpPower = 2f; // ジャンプの高さ
        public int NumJumps = 1; // ジャンプの回数

        protected override Tween CreateTween()
            => Target.TweenLocalJump(Target.localPosition, JumpPower, NumJumps, Duration);
    }
}