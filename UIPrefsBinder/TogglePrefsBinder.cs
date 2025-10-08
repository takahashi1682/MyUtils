using UnityEngine;
using UnityEngine.UI;

namespace MyUtils.UIPrefsBinder
{
    /// <summary>
    ///  Toggle のオンオフ状態を PlayerPrefs に保存・復元するコンポーネント
    /// </summary>
    public class TogglePrefsBinder : AbstractTargetBehaviour<Toggle>
    {
        [SerializeField] private string _prefsKey = "key_toggle";
        [SerializeField] private bool _defaultValue;

        protected override void Awake()
        {
            base.Awake();

            int savedValue = PlayerPrefs.GetInt(_prefsKey, _defaultValue ? 1 : 0);
            _target.isOn = savedValue != 0;

            _target.onValueChanged.AddListener(OnToggleValueChanged);
        }

        private void OnToggleValueChanged(bool newValue)
        {
            PlayerPrefs.SetInt(_prefsKey, newValue ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
}