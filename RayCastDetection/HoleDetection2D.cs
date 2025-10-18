using MyUtils.RayCastDetection.Core;
using R3;

namespace MyUtils.RayCastDetection
{
    /// <summary>
    ///  穴検知
    /// </summary>
    public class HoleDetection2D : BoxCast2dDetection
    {
        public ReadOnlyReactiveProperty<bool> IsHole =>
            Hit2D.Select(v => v.collider == null).ToReadOnlyReactiveProperty();
    }
}