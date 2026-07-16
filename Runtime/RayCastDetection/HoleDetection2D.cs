using MyUtils.RayCastDetection.Core;
using R3;

namespace MyUtils.RayCastDetection
{
    /// <summary>
    ///  穴検知
    /// </summary>
    public class HoleDetection2D : BoxCast2dDetection
    {
        private ReadOnlyReactiveProperty<bool> _isHole;
        public ReadOnlyReactiveProperty<bool> IsHole =>
            _isHole ??= Hit2D.Select(v => v.collider == null).ToReadOnlyReactiveProperty().AddTo(this);
    }
}