using System;
using MyUtils.AudioManager.Core;
using R3;
using UnityEngine;
using UnityEngine.Audio;
using MyUtils.Abstract;

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

        [Header("保存設定")]
        [Tooltip("音量をPlayerPrefsに保存し、次回起動時に復元する")]
        [SerializeField] private bool _saveVolume = true;
        [Tooltip("PlayerPrefsのキーの先頭に付ける文字(例: Volume_Master)")]
        [SerializeField] private string _prefsKeyPrefix = "Volume_";

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

            // 重複したインスタンスは破棄されるので、保存の対象にしない
            if (_saveVolume && Instance == this)
            {
                LoadAndAutoSaveVolumes();
            }
        }

        protected override void OnDestroy()
        {
            if (_saveVolume && Instance == this) PlayerPrefs.Save();

            base.OnDestroy();
        }

        // 保存済みの音量を復元し、以降は音量が変わるたびにPlayerPrefsへ書き込む(ディスクへの書き込みは終了時)
        private void LoadAndAutoSaveVolumes()
        {
            foreach (EAudioMixerParam param in Enum.GetValues(typeof(EAudioMixerParam)))
            {
                var key = _prefsKeyPrefix + param;
                var rate = VolumeRates[param];

                if (PlayerPrefs.HasKey(key))
                {
                    rate.Value = Mathf.Clamp01(PlayerPrefs.GetFloat(key));
                }

                rate.Skip(1)
                    .Subscribe(v => PlayerPrefs.SetFloat(key, v))
                    .AddTo(this);
            }
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