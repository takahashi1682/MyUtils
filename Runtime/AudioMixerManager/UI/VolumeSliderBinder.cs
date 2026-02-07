using R3;
using UnityEngine;
using UnityEngine.UI;

namespace MyUtils.AudioMixerManager.UI
{
    /// <summary>
    /// 音量調整スライダーの値をAudioMixerManagerと同期する
    /// </summary>
    public class VolumeSliderBinder : AbstractVolumeBinder
    {
        [SerializeField] private Slider _volumeSlider;

        protected override void Start()
        {
            base.Start();
            InitializeSlider();
        }

        private void InitializeSlider()
        {
            if (_volumeSlider == null) return;

            // Sliderの初期化
            _volumeSlider.minValue = 0;
            _volumeSlider.maxValue = _maxVolumeLevel;
            _volumeSlider.SetValueWithoutNotify(VolumeLevel.Value);
            _volumeSlider.wholeNumbers = true;

            // Slider操作時の値を反映
            _volumeSlider
                .OnValueChangedAsObservable()
                .Subscribe(x => VolumeLevel.Value = x)
                .AddTo(this);

            // VolumeLevelの値をSliderに反映
            VolumeLevel
                .Subscribe(x => _volumeSlider.value = x)
                .AddTo(this);
        }
    }
}