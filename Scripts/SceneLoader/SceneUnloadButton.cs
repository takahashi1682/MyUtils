using System.Threading;
using Cysharp.Threading.Tasks;
using TUtils.UICommon;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TUtils.SceneLoader
{
    /// <summary>
    /// シーンのアンロードボタン
    /// </summary>
    public class SceneUnloadButton : AbstractButton
    {
        protected override async UniTask OnClick(CancellationToken cts)
            => await SceneManager.UnloadSceneAsync(gameObject.scene.name);
    }
}