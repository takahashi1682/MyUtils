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

        protected override void Start()
        {
            base.Start();

            int savedValue = PlayerPrefs.GetInt(_prefsKey, _defaultValue);
            Target.value = Mathf.Clamp(savedValue, 0, Target.options.Count - 1);

            Target.onValueChanged.AddListener(OnDropdownValueChanged);
        }

        private void OnDropdownValueChanged(int newValue)
        {
            PlayerPrefs.SetInt(_prefsKey, newValue);
            PlayerPrefs.Save();
        }
    }
}