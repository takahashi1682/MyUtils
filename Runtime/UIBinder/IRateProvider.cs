using R3;

namespace MyUtils.UIBinder
{
    public interface IRateProvider
    {
        ReadOnlyReactiveProperty<float> CurrentRate { get; }
    }
}