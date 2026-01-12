using System.Threading;
using Cysharp.Threading.Tasks;
using MyUtils.FadeScreen;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyUtils.SceneLoader
{
    public class SceneLoaderInputTrigger : AbstractInputTrigger
    {
        [SerializeField] private SceneReference _nextScene;
        [SerializeField] private LoadSceneMode _loadSceneMode = LoadSceneMode.Single;

        [Header("ロード画面設定")]
        [SerializeField] private bool _isUseLoadingScene;
        [SerializeField] private SceneReference _loadingScene;
        [SerializeField] private int _minLoadingTime = 2;

        [Header("フェード設定")]
        [SerializeField] private bool _isFade = true;
        [SerializeField] private FadeSetting _fadeSetting;

        protected override async UniTask OnPress(CancellationToken ct)
        {
            if (SceneManager.GetActiveScene().name == _nextScene.SceneName) return;

            string nextScene = _nextScene.SceneName;
            var fadeSetting = _isFade ? _fadeSetting : null;
            if (_isUseLoadingScene)
            {
                // ロード画面の読み込み
                await SceneLoaderUtils.LoadSceneAsync(_loadingScene.SceneName, fadeSetting);

                // 目的のシーンの読み込み
                await SceneLoaderUtils.LoadSceneAsync(nextScene, fadeSetting,
                    minLoadingTime: _minLoadingTime);
            }
            else
            {
                await SceneLoaderUtils.LoadSceneAsync(nextScene, fadeSetting, _loadSceneMode);
            }
        }
    }
}