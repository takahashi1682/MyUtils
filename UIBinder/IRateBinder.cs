using R3;

namespace MyUtils.UIBinder
{
    public interface IRateBinder
    {
        ReadOnlyReactiveProperty<float> CurrentRate { get; }
    }
}