using UnityEngine;

namespace MyUtils.TweenUtils
{
    /// <summary>
    ///     シンプルな移動機能
    /// </summary>
    [RequireComponent(typeof(Transform))]
    public class TweenLocalMove : AbstractTween<Transform>
    {
        [Header("TweenLocalMove")]
        public bool IsRelative;
        public Vector3 EndValue = new(300, 0, 0);
        
        protected override Tween CreateTween()
            => Target.TweenLocalMove(EndValue, Duration).SetRelative(IsRelative);
    }
}