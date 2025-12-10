using System;
using UnityEngine;
using UnityEngine.Audio;

namespace MyUtils.AudioManager.Core
{
    [Serializable]
    public class AudioSetting
    {
        [SerializeField] // 初期値を反映するために必要
        public float Volume = 1f;
        public AudioResource Resource;
        public bool IsLoop;

        public AudioSetting()
        {
        }

        public AudioSetting(AudioResource resource, float volume = 1f, bool isLoop = false)
        {
            Resource = resource;
            Volume = volume;
            IsLoop = isLoop;
        }
    }
}