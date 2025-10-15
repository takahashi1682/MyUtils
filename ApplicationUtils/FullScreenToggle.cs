using R3;
using UnityEngine.UI;
using Screen = UnityEngine.Device.Screen;

namespace MyUtils.ApplicationUtils
{
    public class FullScreenToggle : AbstractTargetBehaviour<Toggle>
    {
        private readonly ReactiveProperty<bool> _isFullScreen = new(Screen.fullScreen);

        protected override void Start()
        {
            base.Start();

            _isFullScreen.AddTo(this);
            _isFullScreen.Subscribe(x => Target.isOn = x).AddTo(this);

            Target.OnValueChangedAsObservable()
                .Skip(1)
                .Where(isOn => isOn != Screen.fullScreen)
                .Subscribe(isOn => Screen.fullScreen = isOn)
                .AddTo(this);
        }

        private void Update() => _isFullScreen.Value = Screen.fullScreen;
    }
}