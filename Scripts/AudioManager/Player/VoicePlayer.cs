using MyUtils.AudioManager.Core;
using MyUtils.AudioManager.Manager;

namespace MyUtils.AudioManager.Player
{
    /// <summary>
    /// Voice再生機能
    /// </summary>
    public class VoicePlayer : AbstractPlayer
    {
        public override AudioPlayer Play() => VoiceManager.Play(Setting);
    }
}