using DG.Tweening;
using UnityEngine;

namespace MyUtils.DOTweenUtils
{
    public class DoJump : AbstractDoTween<Transform>
    {
        public float JumpPower = 2f; // ジャンプの高さ
        public int NumJumps = 1; // ジャンプの回数

        protected override Tween CreateTween()
            => _target.DOLocalJump(_target.localPosition, JumpPower, NumJumps, Duration);
    }
}