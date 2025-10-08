using MyUtils.AudioManager.Core;
using MyUtils.AudioManager.Manager;

namespace MyUtils.AudioManager.Player
{
    /// <summary>
    /// SE再生機能
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class SEPlayer : AbstractPlayer
    {
        protected override AudioPlayer Play() => SEManager.Play(Setting);
    }
}