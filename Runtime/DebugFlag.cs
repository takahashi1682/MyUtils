#if UNITY_EDITOR
using UnityEngine;
#endif

namespace MyUtils.Misc
{
    /// <summary>
    /// エディタのウィンドウ(Window/MyUtils/Debug Flag Window)で切り替える、50個のデバッグフラグ。
    /// スクリプトからは if (DebugFlag.IsOn(1)) の1行で使う(番号は1〜50)。ビルドでは常に false になる。
    /// </summary>
    public static class DebugFlag
    {
        public const int Count = 50;

#if UNITY_EDITOR
        private static readonly bool?[] Flags = new bool?[Count];

        // EditorPrefs はPC全体で共有されるので、プロジェクトごとに分ける
        private static string PrefsKey(int no) => $"{Application.productName}.MyUtils.DebugFlag{no}";

        // 毎フレーム呼ばれてもよいように、最初の1回だけ EditorPrefs から読む
        public static bool IsOn(int no) => Flags[no - 1] ??= UnityEditor.EditorPrefs.GetBool(PrefsKey(no));

        public static void Set(int no, bool isOn)
        {
            Flags[no - 1] = isOn;
            UnityEditor.EditorPrefs.SetBool(PrefsKey(no), isOn);
        }
#else
        public static bool IsOn(int no) => false;
#endif
    }
}
