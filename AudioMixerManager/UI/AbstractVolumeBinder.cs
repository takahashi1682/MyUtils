using MyUtils.AudioManager.Core;
using R3;
using UnityEngine;

namespace MyUtils.AudioMixerManager.UI
{
    public abstract class AbstractVolumeBinder : MonoBehaviour
    {
        [SerializeField] protected EAudioMixerParam _param;
        [SerializeField] protected int _maxVolumeLevel = 10;
        [field: SerializeField] public SerializableReactiveProperty<float> VolumeLevel = new(10);

        protected AudioVolumeRates _volumeRates;

        protected virtual async void Start()
        {
            VolumeLevel.AddTo(this);

            // AudioMixerManagerの読み込みを待機
            var mixer = await AudioMixerManager.InitializeAsync;
            _volumeRates = mixer.VolumeRates;

            // 初期化
            SubscribeToVolumeRates();
        }

        protected void SubscribeToVolumeRates()
        {
            // AudioMixer側の値をSliderに反映
            _volumeRates[_param]
                .Subscribe(x => VolumeLevel.Value = _maxVolumeLevel * x)
                .AddTo(this);

            // VolumeLevelの値をAudioMixerに反映
            VolumeLevel
                .Subscribe(x => _volumeRates[_param].Value = x / _maxVolumeLevel)
                .AddTo(this);
        }
    }
}