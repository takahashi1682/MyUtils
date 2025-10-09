using MyUtils.DataStore;
using R3;
using UnityEngine;

namespace MyUtils.AudioMixerManager
{
    /// <summary>
    /// ボリュームの保存/読み込みを行う
    /// </summary>
    public class AudioVolumePrefsBinder : MonoBehaviour
    {
        [SerializeField] private AudioMixerManager _audioMixerManager;

        private void Start()
        {
            var setting = PlayerSettingsStore.Singleton.CurrentData;
            _audioMixerManager.VolumeRates[EAudioMixerParam.Master].Value = setting.MasterVolume;
            _audioMixerManager.VolumeRates[EAudioMixerParam.BGM].Value = setting.BGMVolume;
            _audioMixerManager.VolumeRates[EAudioMixerParam.SE].Value = setting.SEVolume;
            _audioMixerManager.VolumeRates[EAudioMixerParam.Voice].Value = setting.VoiceVolume;

            _audioMixerManager.VolumeRates[EAudioMixerParam.Master].Subscribe(x => setting.MasterVolume = x)
                .AddTo(this);
            _audioMixerManager.VolumeRates[EAudioMixerParam.BGM].Subscribe(x => setting.BGMVolume = x).AddTo(this);
            _audioMixerManager.VolumeRates[EAudioMixerParam.SE].Subscribe(x => setting.SEVolume = x).AddTo(this);
            _audioMixerManager.VolumeRates[EAudioMixerParam.Voice].Subscribe(x => setting.VoiceVolume = x).AddTo(this);
        }
    }
}