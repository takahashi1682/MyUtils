using System.Threading;
using R3;
using UnityEngine;

namespace MyUtils.AudioManager.Core
{
    /// <summary>
    /// 各オーディオマネージャーが管理するオーディオプレイヤー
    /// </summary>
    public class AudioPlayer : MonoBehaviour
    {
        public AudioSource AudioSource { get; private set; }
        public ReactiveProperty<float> VolumeRate { get; } = new(1);
        private AudioManager _manager;
        private bool _isPaused;
        public CancellationTokenSource Cts { get; private set; } = new();

        //　使用中かどうか
        public bool IsInUse => AudioSource.isPlaying || _isPaused;

        public void Initialize(AudioManager manager)
        {
            VolumeRate.AddTo(this);
            _manager = manager;

            AudioSource = gameObject.AddComponent<AudioSource>();
            AudioSource.outputAudioMixerGroup = _manager.MixerGroup;
            AudioSource.playOnAwake = false;

            // 音量の更新
            VolumeRate.Subscribe(_ => UpdateVolume()).AddTo(this);
            _manager.VolumeRate.Subscribe(_ => UpdateVolume()).AddTo(this);
        }

        /// <summary>
        /// 再生準備
        /// </summary>
        /// <param name="setting"></param>
        public void Ready(AudioSetting setting)
        {
            if (Cts != null)
            {
                Cts.Cancel();
                Cts.Dispose();
            }

            Cts = new CancellationTokenSource();
            
            VolumeRate.Value = setting.Volume;
            AudioSource.resource = setting.Resource;
            AudioSource.loop = setting.IsLoop;
            transform.position = setting.Position;
            _isPaused = true;
        }

        /// <summary>
        /// 再生
        /// </summary>
        /// <param name="setting"></param>
        public void Play(AudioSetting setting)
        {
            Ready(setting);
            Play();
        }

        /// <summary>
        /// 再生
        /// </summary>
        public void Play()
        {
            AudioSource.Play();
            _isPaused = false;
        }

        /// <summary>
        /// 一時停止解除
        /// </summary>
        public void UnPause()
        {
            AudioSource.UnPause();
            _isPaused = false;
        }

        /// <summary>
        /// 一時停止
        /// </summary>
        public void Pause()
        {
            AudioSource.Pause();
            _isPaused = true;
        }

        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {
            Cts?.Cancel();
            AudioSource.Stop();
            AudioSource.clip = null;
            VolumeRate.Value = 1f;
            _isPaused = false;
        }

        private void UpdateVolume()
            => AudioSource.volume = VolumeRate.Value * _manager.VolumeRate.CurrentValue;
    }
}