using MyUtils.DataStore;
using R3;
using TMPro;

namespace MyUtils.ApplicationUtils
{
    public class ResolutionSelect : AbstractTargetBehaviour<TMP_Dropdown>
    {
        protected override async void Start()
        {
            base.Start();

            var setting = await PlayerSettingsStore.InitializeAsync;
            Target.value = (int)setting.Current.Resolution;

            Target.OnValueChangedAsObservable()
                .Skip(1)
                .Subscribe(SetResolution)
                .AddTo(this);
        }

        public static void SetResolution(int resolutionIndex)
            => ResolutionManager.ApplyResolution(resolutionIndex);
    }
}