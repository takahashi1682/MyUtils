using System.Collections.Generic;
using MyUtils.AudioMixerManager;
using R3;

namespace MyUtils.AudioManager.Core
{
    public class AudioVolumeRates : Dictionary<EAudioMixerParam, ReactiveProperty<float>>
    {
    }
}