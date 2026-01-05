using MyUtils.AudioManager.Core;
using MyUtils.AudioManager.Manager;

namespace MyUtils.AudioManager.Player
{
    /// <summary>
    /// Voice再生機能
    /// </summary>
    public class VoicePlayer : AbstractPlayer
    {
        private AudioPlayer _audioPlayer;
        protected override void Play() => _audioPlayer = VoiceManager.Play(Setting);

        protected override void Stop()
        {
            if (_audioPlayer)
            {
                _audioPlayer.Stop();
            }
        }
    }
}