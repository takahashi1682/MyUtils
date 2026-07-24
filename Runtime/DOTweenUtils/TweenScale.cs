using UnityEngine;

namespace MyUtils.DOTweenUtils
{
    public class TweenScale : AbstractDoTween<Transform>
    {
        [Header("TweenScale")] public Vector3 ToScale = new(1, 1.2f, 1);

        protected override Tween CreateTween()
            => Target.DOScale(ToScale, Duration);
    }
}