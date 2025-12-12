using System.Globalization;
using Cysharp.Threading.Tasks;
using MyUtils.AudioManager.Core;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace MyUtils.AudioMixerManager.UI
{
    /// <summary>
    /// 音量調整スライダーの値をAudioMixerManagerと同期する
    /// </summary>
    public class VolumeSliderBinder : MonoBehaviour
    {
        [SerializeField] private EAudioMixerParam _param;
        [SerializeField] private int _maxVolumeLevel = 10;
        [field: SerializeField] public SerializableReactiveProperty<float> VolumeLevel = new(10);
        [SerializeField] private Slider _volumeSlider;
        //   [SerializeField] private TMPro.TMP_InputField _volumeInputField;

        private AudioVolumeRates _volumeRates;

        private async void Start()
        {
            VolumeLevel.AddTo(this);

            var mixerManager = AudioMixerManager.Singleton;
            var token = this.GetCancellationTokenOnDestroy();
            _volumeRates = await mixerManager.OnLoadAsObservable.Task.AttachExternalCancellation(token);

            // 初期化
            //InitializeVolumeLevel();
            InitializeSlider();
            // InitializeInputField();
            SubscribeToVolumeLevelChanges();
        }

        // private void InitializeVolumeLevel()
        // {
        //     float volumeRate = _volumeRates[_param].CurrentValue;
        //     Debug.Log(volumeRate);
        //     VolumeLevel.Value = _maxVolumeLevel * volumeRate;
        // }

        private void InitializeSlider()
        {
            if (_volumeSlider == null) return;

            // Sliderの初期化
            _volumeSlider.minValue = 0;
            _volumeSlider.maxValue = _maxVolumeLevel;
            _volumeSlider.SetValueWithoutNotify(VolumeLevel.Value);
            _volumeSlider.wholeNumbers = true;

            _volumeRates[_param].Subscribe(x =>
            {
                VolumeLevel.Value = _maxVolumeLevel * x;
            }).AddTo(this);

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

        private void SubscribeToVolumeLevelChanges()
            => VolumeLevel.Subscribe(x => _volumeRates[_param].Value = x / _volumeSlider.maxValue).AddTo(this);

        // private void InitializeInputField()
        // {
        //     if (_volumeInputField == null) return;
        //
        //     // InputFieldの初期化
        //     _volumeInputField.contentType = TMPro.TMP_InputField.ContentType.IntegerNumber;
        //     _volumeInputField.characterLimit = _maxVolumeLevel.ToString().Length;
        //     _volumeInputField.SetTextWithoutNotify(VolumeLevel.CurrentValue.ToString(CultureInfo.CurrentCulture));
        //
        //     // InputField操作時の値を反映
        //     _volumeInputField
        //         .onEndEdit.AsObservable()
        //         .Select(x =>
        //             // 入力された文字列をfloatに変換し範囲内に収める：範囲外の場合は0にする
        //             float.TryParse(x, out var parsedValue) ? Mathf.Clamp(parsedValue, 0, _maxVolumeLevel) : 0)
        //         .Subscribe(x =>
        //             {
        //                 _volumeInputField.text = x.ToString(CultureInfo.CurrentCulture);
        //                 VolumeLevel.Value = x;
        //             }
        //         );
        //
        //     // VolumeLevelの値をInputFieldに反映
        //     VolumeLevel.Subscribe(x => _volumeInputField.text = x.ToString(CultureInfo.CurrentCulture)).AddTo(this);
        // }
    }
}