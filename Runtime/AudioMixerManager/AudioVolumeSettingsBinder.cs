using System;
using MyUtils.DataStore;
using MyUtils.DataStore.PlayerSetting;
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
            var setting = await PlayerSettingStore.WaitInstanceAsync;
            float[] volumes = setting.Volumes;

            foreach (EAudioMixerParam pram in Enum.GetValues(typeof(EAudioMixerParam)))
            {
                _manager.VolumeRates[pram].Value = volumes[(int)pram]; // 読み込み
                _manager.VolumeRates[pram].Subscribe(v => volumes[(int)pram] = v).AddTo(this); // 書き込み
            }
        }
    }
}