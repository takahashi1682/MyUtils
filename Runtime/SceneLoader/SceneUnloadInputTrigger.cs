using System.Threading;
using Cysharp.Threading.Tasks;
using MyUtils.FadeScreen;
using MyUtils.InputTrigger;
using UnityEngine;

namespace MyUtils.SceneLoader
{
    public class SceneUnloadInputTrigger : AbstractInputTrigger
    {
        [SerializeField] private SceneReference.SceneReference _unloadScene;

        [Header("フェード設定")]
        [SerializeField] private bool _isFade = true;
        [SerializeField] private FadeSetting _fadeSetting;

        protected override async UniTask OnPressed(CancellationToken ct)
        {
            var fadeSetting = _isFade ? _fadeSetting : null;
            await SceneLoaderUtils.UnloadSceneAsync(_unloadScene.SceneName, fadeSetting);
        }
    }
}