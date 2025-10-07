using System.Threading;
using Cysharp.Threading.Tasks;
using MyUtils.UICommon;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyUtils.SceneLoader
{
    public class SceneUnLoadInputTrigger : AbstractInputTrigger
    {
        protected override async UniTask OnPress(CancellationToken ct)
            => await SceneManager.UnloadSceneAsync(gameObject.scene.name);
    }
}