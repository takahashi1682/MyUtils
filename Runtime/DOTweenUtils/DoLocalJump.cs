using DG.Tweening;
using UnityEngine;

namespace MyUtils.DOTweenUtils
{
    public class DoLocalJump : AbstractDoTween<Transform>
    {
        public float JumpPower = 2f; // ジャンプの高さ
        public int NumJumps = 1; // ジャンプの回数

        protected override Tween CreateTween()
            => Target.DOLocalJump(Target.localPosition, JumpPower, NumJumps, Duration);
    }
}