using System.Globalization;
using R3;
using UnityEngine;

namespace MyUtils.AudioMixerManager.UI
{
    /// <summary>
    ///  音量調整入力フィールドの値をAudioMixerManagerと同期する
    /// </summary>
    public class VolumeInputFieldBinder : AbstractVolumeBinder
    {
        [SerializeField] private TMPro.TMP_InputField _volumeField;

        protected override void Start()
        {
            base.Start();
            InitializeInputField();
        }


        private void InitializeInputField()
        {
            if (_volumeField == null) return;

            // InputFieldの初期化
            _volumeField.contentType = TMPro.TMP_InputField.ContentType.IntegerNumber;
            _volumeField.characterLimit = _maxVolumeLevel.ToString().Length;
            _volumeField.SetTextWithoutNotify(VolumeLevel.CurrentValue.ToString(CultureInfo.CurrentCulture));

            // InputField操作時の値を反映
            _volumeField
                .onEndEdit.AsObservable()
                .Subscribe(inputStr =>
                    {
                        float inputFloat = float.TryParse(inputStr, out float parsedValue)
                            ? Mathf.Clamp(parsedValue, 0, _maxVolumeLevel)
                            : 0;

                        _volumeField.text = inputFloat.ToString(CultureInfo.CurrentCulture);
                        VolumeLevel.Value = inputFloat;
                    }
                );

            // VolumeLevelの値をInputFieldに反映
            VolumeLevel
                .Subscribe(x => _volumeField.text = x.ToString(CultureInfo.CurrentCulture))
                .AddTo(this);
        }
    }
}