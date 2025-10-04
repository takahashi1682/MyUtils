using System;
using UnityEngine;
using UnityEngine.Audio;

namespace TUtils.AudioManager
{
    [Serializable]
    public class AudioSetting
    {
        public AudioResource Resource;
        public float Volume = 1f;
        public bool IsLoop;
        public Vector3 Position;

        public AudioSetting()
        {
        }

        public AudioSetting(AudioResource resource, float volume = 1f, bool isLoop = false, Vector3 position = default)
        {
            Resource = resource;
            Volume = volume;
            IsLoop = isLoop;
            Position = position;
        }
    }
}