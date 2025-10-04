using System;
using System.Collections.Generic;
using R3;
using UnityEngine;
using UnityEngine.Audio;

namespace TUtils.AudioMixerManager
{
    /// <summary>
    /// ゲームで使用するAudioMixerの管理
    /// </summary>
    public class AudioMixerManager : AbstractSingletonBehaviour<AudioMixerManager>
    {
        [field: SerializeField] public AudioMixer AudioMixer { get; private set; }
        [SerializeField] private SerializableReactiveProperty<float> _masterVolumeRate = new(1);
        [SerializeField] private SerializableReactiveProperty<float> _bgmVolumeRate = new(1);
        [SerializeField] private SerializableReactiveProperty<float> _seVolumeRate = new(1);
        [SerializeField] private SerializableReactiveProperty<float> _voiceVolumeRate = new(1);
        public readonly Dictionary<EAudioMixerParameters, ReactiveProperty<float>> VolumeRates = new();

        protected override void Awake()
        {
            base.Awake();

            _masterVolumeRate.AddTo(this);
            _bgmVolumeRate.AddTo(this);
            _seVolumeRate.AddTo(this);
            _voiceVolumeRate.AddTo(this);

            VolumeRates[EAudioMixerParameters.Master] = _masterVolumeRate;
            VolumeRates[EAudioMixerParameters.BGM] = _bgmVolumeRate;
            VolumeRates[EAudioMixerParameters.SE] = _seVolumeRate;
            VolumeRates[EAudioMixerParameters.Voice] = _voiceVolumeRate;
        }

        private void Start()
        {
            var parameters = Enum.GetValues(typeof(EAudioMixerParameters));
            foreach (EAudioMixerParameters parameter in parameters)
            {
                SubscribeToVolumeRate(parameter);
            }
        }

        private void SubscribeToVolumeRate(EAudioMixerParameters parameters) =>
            VolumeRates[parameters]
                .Subscribe(v =>
                {
                    AudioMixer.SetFloat(parameters.ToString(), ToDecibelRate(v));
                })
                .AddTo(this);

        /// <summary>
        /// 0~1の音量をデシベルに変換
        /// </summary>
        /// <param name="rate"></param>
        /// <returns></returns>
        public static float ToDecibelRate(float rate)
        {
            var adjustedRate = (float)(1 - Math.Pow(1 - rate, 2));
            return Mathf.Lerp(-80, 0, adjustedRate);
        }
    }
}