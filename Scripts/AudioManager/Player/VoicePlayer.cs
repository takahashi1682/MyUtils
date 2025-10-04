using TUtils.AudioManager.Core;
using TUtils.AudioManager.Manager;

namespace TUtils.AudioManager.Player
{
    /// <summary>
    /// Voice再生機能
    /// </summary>
    public class VoicePlayer : AbstractPlayer
    {
        public override AudioPlayer Play() => VoiceManager.Play(Setting);
    }
}