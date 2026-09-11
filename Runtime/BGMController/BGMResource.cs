using MyUtils.AudioManager.Core;
using UnityEngine;

namespace MyUtils
{
    [CreateAssetMenu(fileName = "BGMResource", menuName = "MyUtils/AudioManager/BGMResource")]
    public class BGMResource : ScriptableObject
    {
        public AudioClip Clip;

        [Tooltip("ループ再生するかどうか")]
        public bool IsLoop = true;

        [Tooltip("優先度 0=最高, 256=最低")]
        [Range(0, 256)] public int Priority = 128;

        [Tooltip("音量 0=無音, 1=最大")]
        [Range(0, 1)] public float Volume = 1f;

        [Tooltip("ピッチ調整")]
        [Range(-3f, 3f)] public float Pitch = 1f;

        public AudioSetting ToAudioSetting()
        {
            return new AudioSetting
            {
                Clip = Clip,
                IsLoop = IsLoop,
                Priority = Priority,
                Volume = Volume,
                Pitch = Pitch
            };
        }
    }
}