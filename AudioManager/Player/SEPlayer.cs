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
        private AudioPlayer _audioPlayer;
        protected override void Play() => _audioPlayer = SEManager.Play(Setting);

        protected override void Stop()
        {
            if (_audioPlayer)
            {
                _audioPlayer.Stop();
            }
        }
    }
}