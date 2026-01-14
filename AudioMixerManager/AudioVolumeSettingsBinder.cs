using System;
using MyUtils.DataStore;
using R3;
using UnityEngine;

namespace MyUtils.AudioMixerManager
{
    /// <summary>
    ///   AudioMixerManagerのボリューム設定をPlayerSettingと同期する
    /// </summary>
    public class AudioVolumeSettingsBinder : MonoBehaviour
    {
        [SerializeField] private AudioMixerManager _manager;

        private async void Start()
        {
            var setting = await PlayerSettingsStore.AsyncInstance;
            float[] volumes = setting.Current.Volumes;

            foreach (EAudioMixerParam pram in Enum.GetValues(typeof(EAudioMixerParam)))
            {
                _manager.VolumeRates[pram].Value = volumes[(int)pram]; // 読み込み
                _manager.VolumeRates[pram].Subscribe(v => volumes[(int)pram] = v).AddTo(this); // 書き込み
            }
        }
    }
}