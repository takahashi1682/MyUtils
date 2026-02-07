using MyUtils.RayCastDetection.Core;
using R3;

namespace MyUtils.RayCastDetection
{
    /// <summary>
    ///  壁検知
    /// </summary>
    public class WallDetection2D : BoxCast2dDetection
    {
        public ReadOnlyReactiveProperty<bool> IsWall =>
            Hit2D.Select(v => v.collider != null).ToReadOnlyReactiveProperty();
    }
}