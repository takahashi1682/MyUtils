using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MyUtils.AudioManager.Core;
using MyUtils.AudioManager.Manager;
using UnityEngine;

namespace MyUtils.AudioManager.Player
{
    /// <summary>
    /// BGM再生機能
    /// </summary>
    public class IntroLoopBGMPlayer : AbstractPlayer
    {
        public AudioClip IntroClip;
        public AudioClip LoopClip;
        public float Volume = 1;
        private AudioPlayer _audioPlayer;
        private readonly CancellationTokenSource _cancellationToken = new();

        protected override void Start()
        {
            // 事前にAudioClipのデータを読み込んでおく
            IntroClip.LoadAudioData();
            LoopClip.LoadAudioData();
           
            base.Start();
        }

        protected override async void Play()
        {
            _audioPlayer = BGMManager.Play(IntroClip, Volume);

            await UniTask.Delay(TimeSpan.FromSeconds(IntroClip.length),
                DelayType.Realtime,
                cancellationToken: _cancellationToken.Token);

            _audioPlayer = BGMManager.Play(LoopClip, Volume, true);
        }

        protected override void Stop()
        {
            if (_audioPlayer)
            {
                _audioPlayer.Stop();
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _cancellationToken.Cancel();
        }
    }
}