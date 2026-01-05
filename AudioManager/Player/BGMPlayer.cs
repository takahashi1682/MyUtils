using MyUtils.AudioManager.Core;
using MyUtils.AudioManager.Manager;

namespace MyUtils.AudioManager.Player
{
    /// <summary>
    ///  BGM再生機能
    /// </summary>
    public class BGMPlayer : AbstractPlayer
    {
        public AudioSetting Setting;
        public override void Play() => _audioPlayer = BGMManager.Play(Setting, transform);
    }
}