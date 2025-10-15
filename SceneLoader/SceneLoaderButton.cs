using System.Threading;
using Cysharp.Threading.Tasks;
using MyUtils.FadeScreen;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyUtils.SceneLoader
{
    /// <summary>
    /// シーンの読み込みボタン
    /// </summary>
    public class SceneLoaderButton : AbstractAwaitButton
    {
        [SerializeField] private ESceneName _nextScene;
        [SerializeField] private LoadSceneMode _loadSceneMode = LoadSceneMode.Single;

        [Header("ロード画面設定")]
        [SerializeField] private bool _isUseLoadingScene;
        [SerializeField] private ESceneName _loadingScene = ESceneName.Loading;
        [SerializeField] private int _minLoadingTime = 3;

        [Header("フェード設定")]
        [SerializeField] private bool _isFade = true;
        [SerializeField] private FadeSetting _fadeSetting;

        protected override async UniTask OnClick(CancellationToken cts)
        {
            var fadeSetting = _isFade ? _fadeSetting : null;
            if (_isUseLoadingScene)
            {
                // ロード画面の読み込み
                await SceneLoaderUtils.LoadSceneAsync(_loadingScene, fadeSetting);

                // 目的のシーンの読み込み
                await SceneLoaderUtils.LoadSceneAsync(_nextScene, fadeSetting,
                    minLoadingTime: _minLoadingTime);
            }
            else
            {
                await SceneLoaderUtils.LoadSceneAsync(_nextScene, fadeSetting, _loadSceneMode);
            }
        }
    }
}