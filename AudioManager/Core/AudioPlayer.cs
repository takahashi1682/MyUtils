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
        public CancellationTokenSource Cts { get; private set; } = new();

        private Transform _trackingTarget;
        private AudioManager _manager;
        private bool _isPaused;

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
        /// <param name="trackingTarget"></param>
        public void Ready(AudioSetting setting, Transform trackingTarget)
        {
            if (Cts != null)
            {
                Cts.Cancel();
                Cts.Dispose();
            }

            Cts = new CancellationTokenSource();

            SetAudioSetting(setting);
            _trackingTarget = trackingTarget;
            _isPaused = true;
        }

        private void SetAudioSetting(AudioSetting setting)
        {
            AudioSource.resource = setting.Clip;
            AudioSource.mute = setting.Mute;
            AudioSource.bypassEffects = setting.BypassEffects;
            AudioSource.bypassListenerEffects = setting.BypassListenerEffects;
            AudioSource.bypassReverbZones = setting.BypassReverbZones;
            AudioSource.loop = setting.IsLoop;
            VolumeRate.Value = setting.Volume;
            AudioSource.priority = setting.Priority;
            AudioSource.pitch = setting.Pitch;
            AudioSource.panStereo = setting.PanStereo;
            AudioSource.spatialBlend = setting.SpatialBlendType == SpatialBlendType._2D ? 0f : 1f;
            AudioSource.reverbZoneMix = setting.ReverbZoneMix;
            AudioSource.dopplerLevel = setting.DopplerLevel;
            AudioSource.spread = setting.Spread;
            AudioSource.rolloffMode = setting.RolloffMode;
            AudioSource.minDistance = setting.MinDistance;
            AudioSource.maxDistance = setting.MaxDistance;
        }

        /// <summary>
        /// 再生
        /// </summary>
        /// <param name="setting"></param>
        /// <param name="trackingTarget"></param>
        public void Play(AudioSetting setting, Transform trackingTarget)
        {
            Ready(setting, trackingTarget);
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
            _trackingTarget = null;
            AudioSource.clip = null;
            _isPaused = false;
        }

        private void UpdateVolume()
            => AudioSource.volume = VolumeRate.Value * _manager.VolumeRate.CurrentValue;

        private void FixedUpdate()
        {
            if (_trackingTarget == null) return;
            if (!AudioSource.isPlaying) return;

            transform.position = _trackingTarget.position;
        }
    }
}