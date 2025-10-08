#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

// 消すな

namespace MyUtils
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