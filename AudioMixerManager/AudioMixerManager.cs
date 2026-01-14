using System;
using MyUtils.AudioManager.Core;
using R3;
using UnityEngine;
using UnityEngine.Audio;

namespace MyUtils.AudioMixerManager
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
        public readonly AudioVolumeRates VolumeRates = new();

        protected override void Awake()
        {
            _masterVolumeRate.AddTo(this);
            _bgmVolumeRate.AddTo(this);
            _seVolumeRate.AddTo(this);
            _voiceVolumeRate.AddTo(this);

            VolumeRates[EAudioMixerParam.Master] = _masterVolumeRate;
            VolumeRates[EAudioMixerParam.BGM] = _bgmVolumeRate;
            VolumeRates[EAudioMixerParam.SE] = _seVolumeRate;
            VolumeRates[EAudioMixerParam.Voice] = _voiceVolumeRate;

            base.Awake();
        }

        private void Start()
        {
            var parameters = Enum.GetValues(typeof(EAudioMixerParam));
            foreach (EAudioMixerParam parameter in parameters)
            {
                SubscribeToVolumeRate(parameter);
            }
        }

        private void SubscribeToVolumeRate(EAudioMixerParam param) =>
            VolumeRates[param]
                .Subscribe(v =>
                {
                    AudioMixer.SetFloat(param.ToString(), ToDecibelRate(v));
                })
                .AddTo(this);

        /// <summary>
        /// 0~1の音量をデシベルに変換
        /// </summary>
        /// <param name="rate"></param>
        /// <returns></returns>
        public static float ToDecibelRate(float rate)
        {
            float adjustedRate = (float)(1 - Math.Pow(1 - rate, 2));
            return Mathf.Lerp(-80, 0, adjustedRate);
        }
    }
}