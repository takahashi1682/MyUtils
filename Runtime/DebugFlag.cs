#if UNITY_EDITOR
using UnityEngine;
#endif

namespace MyUtils.Misc
{
    /// <summary>
    /// エディタのメニュー(Window/MyUtils/Debug Mode)で切り替えるデバッグフラグ。
    /// スクリプトからは if (DebugFlag.IsOn) の1行で使う。ビルドでは常に false になる。
    /// </summary>
    public static class DebugFlag
    {
#if UNITY_EDITOR
        private static bool? _isOn;

        // EditorPrefs はPC全体で共有されるので、プロジェクトごとに分ける
        private static string PrefsKey => $"{Application.productName}.MyUtils.DebugFlag";

        public static bool IsOn
        {
            // 毎フレーム呼ばれてもよいように、最初の1回だけ EditorPrefs から読む
            get => _isOn ??= UnityEditor.EditorPrefs.GetBool(PrefsKey);
            set
            {
                _isOn = value;
                UnityEditor.EditorPrefs.SetBool(PrefsKey, value);
            }
        }
#else
        // const にすると、呼び出し側で「到達できないコード」の警告が出るので、プロパティにする
        public static bool IsOn => false;
#endif
    }
}
