using TMPro;
using UnityEngine;

namespace MyUtils.UIPrefsBinder
{
    /// <summary>
    ///  Dropdown の選択状態を PlayerPrefs に保存・復元するコンポーネント
    /// </summary>
    public class DropdownPrefsBinder : AbstractTargetBehaviour<TMP_Dropdown>
    {
        [SerializeField] private string _prefsKey = "key_dropdown";
        [SerializeField] private int _defaultValue;

        protected override void Awake()
        {
            base.Awake();

            int savedValue = PlayerPrefs.GetInt(_prefsKey, _defaultValue);
            _target.value = Mathf.Clamp(savedValue, 0, _target.options.Count - 1);

            _target.onValueChanged.AddListener(OnDropdownValueChanged);
        }

        private void OnDropdownValueChanged(int newValue)
        {
            PlayerPrefs.SetInt(_prefsKey, newValue);
            PlayerPrefs.Save();
        }
    }
}