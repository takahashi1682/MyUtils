using UnityEngine;

namespace MyUtils.TweenUtils
{
    public class TweenScale : AbstractTween<Transform>
    {
        [Header("TweenScale")] public Vector3 ToScale = new(1, 1.2f, 1);

        protected override Tween CreateTween()
            => Target.TweenScale(ToScale, Duration);
    }
}