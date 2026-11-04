#if UNITY_EDITOR
using MyUtils.Misc;
using UnityEditor;

namespace MyUtils
{
    /// <summary>
    /// メニュー(Window/MyUtils/Debug Mode)で DebugFlag を切り替える。オンの間はチェックが付く。
    /// </summary>
    public static class DebugFlagMenu
    {
        private const string MenuPath = "Window/MyUtils/Debug Mode";

        [MenuItem(MenuPath)]
        private static void Toggle() => DebugFlag.IsOn = !DebugFlag.IsOn;

        [MenuItem(MenuPath, true)]
        private static bool Validate()
        {
            Menu.SetChecked(MenuPath, DebugFlag.IsOn);
            return true;
        }
    }
}
#endif
