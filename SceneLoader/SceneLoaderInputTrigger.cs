using System.Threading;
using Cysharp.Threading.Tasks;
using MyUtils.FadeScreen;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyUtils.SceneLoader
{
    public class SceneLoaderInputTrigger : AbstractInputTrigger
    {
        [Header("Scene Loader Settings")]
        [SerializeField] private SceneReference _nextScene;
        [SerializeField] private LoadSceneMode _loadSceneMode = LoadSceneMode.Single;
        [SerializeField] private FadeSetting _fadeSetting;

        protected override async UniTask OnPress(CancellationToken ct)
            => await SceneLoaderUtils.LoadSceneAsync(_nextScene.SceneName, _fadeSetting, _loadSceneMode);
    }
}