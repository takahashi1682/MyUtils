using Config;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyUtils
{
    /// <summary>
    /// ゲーム起動時に必要なプレハブを含んだシーンを生成する
    /// </summary>
    public class StartupSceneLoader : MonoBehaviour
    {
        // ゲーム起動時にStartupシーンを読み込む
        [RuntimeInitializeOnLoadMethod]
        private static void Initialize() =>
            SceneManager.LoadScene(ESceneName.Startup.ToString(), LoadSceneMode.Additive);
    }
}