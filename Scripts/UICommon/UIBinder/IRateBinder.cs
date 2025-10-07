using R3;

namespace MyUtils.UICommon.UIBinder
{
    public interface IRateBinder
    {
        ReadOnlyReactiveProperty<float> CurrentRate { get; }
    }
}