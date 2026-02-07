using System.Threading;
using Cysharp.Threading.Tasks;
using MyUtils.FadeScreen;
using UnityEngine;

namespace MyUtils.SceneLoader
{
    public class SceneUnloadButton : AbstractAsyncButton
    {
        [SerializeField] private SceneReference.SceneReference _unloadScene;

        [Header("フェード設定")]
        [SerializeField] private bool _isFade = true;
        [SerializeField] private FadeSetting _fadeSetting;

        protected override async UniTask OnClick(CancellationToken cts)
        {
            var fadeSetting = _isFade ? _fadeSetting : null;
            await SceneLoaderUtils.UnloadSceneAsync(_unloadScene.SceneName, fadeSetting);
        }
    }
}