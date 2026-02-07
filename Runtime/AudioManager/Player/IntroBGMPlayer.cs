using MyUtils.AudioManager.Core;
using MyUtils.AudioManager.Manager;
using UnityEngine;

namespace MyUtils.AudioManager.Player
{
    /// <summary>
    /// BGM再生機能
    /// </summary>
    public class IntroBGMPlayer : AbstractPlayer
    {
        public AudioSetting IntroSetting;
        public AudioSetting LoopSetting;
        [Tooltip("現在のオーディオエンジンの正確な時間から再生開始までの遅延時間（秒）")]
        public double PlayOffset = 0.1;

        private AudioPlayer _introPlayer;
        private AudioPlayer _loopPlayer;

        protected override void Start()
        {
            // 事前にAudioClipのデータを読み込んでおく
            IntroSetting.Clip.LoadAudioData();
            LoopSetting.Clip.LoadAudioData();

            base.Start();
        }

        public override void Play()
        {
            Stop();

            // 現在のオーディオエンジンの正確な時間 + 0.1秒後に再生開始
            double startTime = AudioSettings.dspTime + PlayOffset;

            // イントロを予約再生
            _introPlayer = BGMManager.Ready(IntroSetting, transform);
            _introPlayer.AudioSource.PlayScheduled(startTime);

            // イントロの長さを計算（秒単位 = サンプル数 ÷ サンプリング周波数）
            double introDuration = (double)IntroSetting.Clip.samples / IntroSetting.Clip.frequency;

            // ループをイントロ終了直後に予約再生
            _loopPlayer = BGMManager.Ready(LoopSetting, transform);
            _loopPlayer.AudioSource.PlayScheduled(startTime + introDuration);
        }

        public override void Pause()
        {
            if (_introPlayer != null)
            {
                _introPlayer.Pause();
            }

            if (_loopPlayer != null)
            {
                _loopPlayer.Pause();
            }
        }

        public override void UnPause()
        {
            if (_introPlayer != null)
            {
                _introPlayer.UnPause();
            }

            if (_loopPlayer != null)
            {
                _loopPlayer.UnPause();
            }
        }

        public override void Stop()
        {
            if (_introPlayer != null)
            {
                _introPlayer.Stop();
                _introPlayer = null;
            }

            if (_loopPlayer != null)
            {
                _loopPlayer.Stop();
                _loopPlayer = null;
            }
        }
    }
}