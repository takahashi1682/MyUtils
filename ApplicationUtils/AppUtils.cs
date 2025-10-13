#if UNITY_EDITOR
using UnityEditor;
#endif
// 消すな

namespace MyUtils.ApplicationUtils
{
    /// <summary>
    ///     アプリケーション関係のUtils
    /// </summary>
    public static class AppUtils
    {
        public static void ExitGame()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}