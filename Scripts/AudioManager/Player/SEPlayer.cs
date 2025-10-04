using TUtils.AudioManager.Core;
using TUtils.AudioManager.Manager;

namespace TUtils.AudioManager.Player
{
    /// <summary>
    /// SE再生機能
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class SEPlayer : AbstractPlayer
    {
        public override AudioPlayer Play() => SEManager.Play(Setting);
    }
}