using MyUtils.AudioManager.Core;
using MyUtils.AudioManager.Manager;

namespace MyUtils.AudioManager.Player
{
    /// <summary>
    /// Voice再生機能
    /// </summary>
    public class VoicePlayer : AbstractPlayer
    {
        protected override AudioPlayer Play() => VoiceManager.Play(Setting);
    }
}