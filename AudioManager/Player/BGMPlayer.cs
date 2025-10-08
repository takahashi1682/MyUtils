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
    public class BGMPlayer : MonoBehaviour
    {
        public AudioClip IntroClip;
        public AudioClip LoopClip;
        public float Volume = 1;
        public bool IsPlayOnAwake = true;
        public bool IsStopOnDestroy = true;

        private AudioPlayer _currentPlayer;
        private readonly CancellationTokenSource _cancellationToken = new();

        private void Start()
        {
            if (IsPlayOnAwake) Play();
        }

        public async void Play()
        {
            if (IntroClip)
            {
                _currentPlayer = BGMManager.Play(IntroClip, Volume);
                await UniTask.Delay(TimeSpan.FromSeconds(IntroClip.length),
                    DelayType.Realtime,
                    cancellationToken: _cancellationToken.Token);
            }

            _currentPlayer = BGMManager.Play(LoopClip, Volume, true);
        }

        private void OnDestroy()
        {
            if (IsStopOnDestroy && _currentPlayer)
            {
                _currentPlayer.Stop();
                _cancellationToken.Cancel();
            }
        }
    }
}