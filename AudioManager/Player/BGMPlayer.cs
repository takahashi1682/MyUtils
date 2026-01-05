using MyUtils.AudioManager.Core;
using MyUtils.AudioManager.Manager;

namespace MyUtils.AudioManager.Player
{
    /// <summary>
    ///  BGM再生機能
    /// </summary>
    public class BGMPlayer : AbstractPlayer
    {
        private AudioPlayer _audioPlayer;
        protected override void Play() => _audioPlayer = BGMManager.Play(Setting);

        protected override void Stop()
        {
            if (_audioPlayer)
            {
                _audioPlayer.Stop();
            }
        }
    }
}