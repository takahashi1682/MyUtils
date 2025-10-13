using UnityEngine;
using UnityEngine.UI;

namespace MyUtils.UIPrefsBinder
{
    /// <summary>
    ///  Slider の値を PlayerPrefs に保存・復元するコンポーネント
    /// </summary>
    public class SliderPrefsBinder : AbstractTargetBehaviour<Slider>
    {
        [SerializeField] private string _prefsKey = "key_slider";
        [SerializeField] private float _defaultValue;

        protected override void Awake()
        {
            base.Awake();

            float savedValue = PlayerPrefs.GetFloat(_prefsKey, _defaultValue);
            Target.value = Mathf.Clamp(savedValue, Target.minValue, Target.maxValue);

            Target.onValueChanged.AddListener(OnFloatValueChanged);
        }

        private void OnFloatValueChanged(float newValue)
        {
            PlayerPrefs.SetFloat(_prefsKey, newValue);
            PlayerPrefs.Save();
        }
    }
}