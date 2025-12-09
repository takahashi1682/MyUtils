using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyUtils.GameManager
{
    /// <summary>
    /// ゲーム起動時にStartupシーンを読み込むクラス
    /// </summary>
    public static class StartupLoader
    {
        private const string SceneName = "Startup";

        [RuntimeInitializeOnLoadMethod]
        private static void Initialize()
        {
            SceneManager.LoadScene(SceneName, LoadSceneMode.Additive);
        }
    }
}