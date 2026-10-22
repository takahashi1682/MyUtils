using UnityEngine;
using R3;
using UnityEngine.UI;
using Screen = UnityEngine.Device.Screen;
using MyUtils.Abstract;

namespace MyUtils.ApplicationUtils
{
    [RequireComponent(typeof(Toggle))]
    public class FullScreenToggle : AbstractTargetBehaviour<Toggle>
    {
        // Screen.fullScreenはコンストラクタ(フィールド初期化子)では呼べないため、Awakeで作る
        private ReactiveProperty<bool> _isFullScreen;

        protected override void Awake()
        {
            base.Awake();
            _isFullScreen = new ReactiveProperty<bool>(Screen.fullScreen);
        }

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