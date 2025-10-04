using System;
using R3;
using UnityEngine;

namespace TUtils.AudioMixerManager
{
    /// <summary>
    /// ボリュームの保存/読み込みを行う
    /// </summary>
    public class AudioVolumePreference : MonoBehaviour
    {
        [SerializeField] private AudioMixerManager _audioMixerManager;
        [SerializeField] private string _registryKey = "AudioVolume_{0}";

        private void Start()
        {
            // AwakeではAudioMixerにアクセスできないためStartで実行
            var parameters = Enum.GetValues(typeof(EAudioMixerParameters));
            foreach (EAudioMixerParameters parameter in parameters)
            {
                LoadVolume(parameter);
                AutoSaveVolume(parameter);
            }
        }

        /// <summary>
        /// 保存された設定値を読み込む
        /// </summary>
        /// <param name="parameter"></param>
        private void LoadVolume(EAudioMixerParameters parameter)
        {
            var loadVolume = PlayerPrefs.GetFloat(string.Format(_registryKey, parameter.ToString()), 1);
            _audioMixerManager.VolumeRates[parameter].Value = loadVolume;
        }

        /// <summary>
        /// ボリューム設定値を自動保存する
        /// </summary>
        /// <param name="parameter"></param>
        private void AutoSaveVolume(EAudioMixerParameters parameter)
            => _audioMixerManager.VolumeRates[parameter]
                .Subscribe(x =>
                {
                    PlayerPrefs.SetFloat(string.Format(_registryKey, parameter.ToString()), x);
                    PlayerPrefs.Save();
                }).AddTo(this);
    }
}