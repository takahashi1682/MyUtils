using System.Threading;
using Cysharp.Threading.Tasks;
using TUtils.UICommon;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TUtils.SceneLoader
{
    public class SceneUnLoadInputTrigger : AbstractInputTrigger
    {
        protected override async UniTask OnPress(CancellationToken ct)
            => await SceneManager.UnloadSceneAsync(gameObject.scene.name);
    }
}