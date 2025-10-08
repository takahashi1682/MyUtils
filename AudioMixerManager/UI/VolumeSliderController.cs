using System.Globalization;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace MyUtils.AudioMixerManager.UI
{
    /// <summary>
    /// 音量調整用のスライダーを制御するクラス
    /// </summary>
    public class VolumeSliderController : MonoBehaviour
    {
        [SerializeField] private EAudioMixerParameters _parameter;
        [SerializeField] private int _maxVolumeLevel = 10;
        [field: SerializeField] public SerializableReactiveProperty<float> VolumeLevel = new(10);
        [SerializeField] private Slider _volumeSlider;
        [SerializeField] private TMPro.TMP_InputField _volumeInputField;

        private AudioMixerManager _mixer;

        private void Start()
        {
            VolumeLevel.AddTo(this);
            _mixer = AudioMixerManager.Instance;

            // 初期化
            InitializeVolumeLevel();
            InitializeSlider();
            InitializeInputField();
            SubscribeToVolumeLevelChanges();
        }

        private void InitializeVolumeLevel()
        {
            var volumeRate = _mixer.VolumeRates[_parameter].Value;
            VolumeLevel.Value = _maxVolumeLevel * volumeRate;
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

        private void InitializeInputField()
        {
            if (_volumeInputField == null) return;

            // InputFieldの初期化
            _volumeInputField.contentType = TMPro.TMP_InputField.ContentType.IntegerNumber;
            _volumeInputField.characterLimit = _maxVolumeLevel.ToString().Length;
            _volumeInputField.SetTextWithoutNotify(VolumeLevel.CurrentValue.ToString(CultureInfo.CurrentCulture));

            // InputField操作時の値を反映
            _volumeInputField
                .onEndEdit.AsObservable()
                .Select(x =>
                    // 入力された文字列をfloatに変換し範囲内に収める：範囲外の場合は0にする
                    float.TryParse(x, out var parsedValue) ? Mathf.Clamp(parsedValue, 0, _maxVolumeLevel) : 0)
                .Subscribe(x =>
                    {
                        _volumeInputField.text = x.ToString(CultureInfo.CurrentCulture);
                        VolumeLevel.Value = x;
                    }
                );

            // VolumeLevelの値をInputFieldに反映
            VolumeLevel.Subscribe(x => _volumeInputField.text = x.ToString(CultureInfo.CurrentCulture)).AddTo(this);
        }

        private void SubscribeToVolumeLevelChanges()
        {
            VolumeLevel.Subscribe(x =>
                _mixer.VolumeRates[_parameter].Value = x / _volumeSlider.maxValue
            ).AddTo(this);
        }
    }
}