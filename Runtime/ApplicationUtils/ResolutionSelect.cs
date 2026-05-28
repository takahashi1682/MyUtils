using R3;
using TMPro;

namespace MyUtils.ApplicationUtils
{
    public class ResolutionSelect : AbstractTargetBehaviour<TMP_Dropdown>
    {
        protected override void Start()
        {
            base.Start();
            Target.OnValueChangedAsObservable()
                .Skip(1)
                .Subscribe(SetResolution)
                .AddTo(this);
        }

        public static void SetResolution(int resolutionIndex)
            => ResolutionApplier.ApplyResolution(resolutionIndex);
    }
}