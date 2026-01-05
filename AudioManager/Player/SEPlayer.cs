using MyUtils.AudioManager.Core;
using MyUtils.AudioManager.Manager;

namespace MyUtils.AudioManager.Player
{
    /// <summary>
    /// SE再生機能
    /// </summary>
    public class SEPlayer : AbstractPlayer
    {
        public AudioSetting Setting;
        public override void Play() => _audioPlayer = SEManager.Play(Setting, transform);
    }
}