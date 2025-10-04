using R3;

namespace TUtils.UICommon.UIBinder
{
    public interface IRateBinder
    {
        ReadOnlyReactiveProperty<float> CurrentRate { get; }
    }
}