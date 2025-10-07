using System.Threading;
using Cysharp.Threading.Tasks;
using MyUtils.UICommon;

namespace MyUtils.SceneLoader
{
    public class GameExitButton : AbstractButton
    {
        protected override UniTask OnClick(CancellationToken ct)
        {
            AppUtils.ExitGame();
            return UniTask.CompletedTask;
        }
    }
}