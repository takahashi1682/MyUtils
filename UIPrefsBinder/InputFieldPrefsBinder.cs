using TMPro;
using UnityEngine;

namespace MyUtils.UIPrefsBinder
{
    /// <summary>
    ///  InputField の入力内容を PlayerPrefs に保存・復元するコンポーネント
    /// </summary>
    public class InputFieldPrefsBinder : AbstractTargetBehaviour<TMP_InputField>
    {
        [SerializeField] private string _prefsKey = "key_inputField";
        [SerializeField] private string _defaultValue;

        protected override void Awake()
        {
            base.Awake();

            string savedValue = PlayerPrefs.GetString(_prefsKey, _defaultValue);
            _target.text = savedValue;

            _target.onValueChanged.AddListener(OnInputFieldValueChanged);
        }

        private void OnInputFieldValueChanged(string newValue)
        {
            PlayerPrefs.SetString(_prefsKey, newValue);
            PlayerPrefs.Save();
        }
    }
}