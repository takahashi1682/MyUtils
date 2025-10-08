using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.Audio;

namespace MyUtils.AudioManager.Core
{
    public abstract class AbstractAudioManager<T> : MonoBehaviour where T : AbstractAudioManager<T>
    {
        [Header("Audio Settings")]
        public int MaxAudioStreams = 10;
        public SerializableReactiveProperty<float> VolumeRate = new(1f);
        public AudioMixerGroup MixerGroup;

        protected static AudioManager Core;
        protected static T Singleton;

        protected virtual void Awake()
        {
            Singleton = (T)this;
            Core = new AudioManager(this, VolumeRate, MixerGroup, MaxAudioStreams);
        }

        protected virtual void OnDestroy()
        {
            Core = null;
            Singleton = null;
        }

        // ==== Wrappers ====

        public static AudioPlayer Play(AudioSetting setting) => Core.Play(setting);

        public static AudioPlayer Play(AudioResource resource, float volume = 1f, bool isLoop = false,
            Vector3 pos = default)
            => Play(new AudioSetting(resource, volume, isLoop, pos));

        public static AudioPlayer HasPlay(AudioSetting setting) => Core.HasPlay(setting);

        public static AudioPlayer HasPlay(AudioResource resource, float volume = 1f, bool isLoop = false,
            Vector3 pos = default)
            => Core.HasPlay(new AudioSetting(resource, volume, isLoop, pos));

        public static AudioPlayer Ready(AudioSetting setting) => Core.Ready(setting);

        public static AudioPlayer Ready(AudioResource resource, float volume = 1f, bool isLoop = false,
            Vector3 pos = default)
            => Ready(new AudioSetting(resource, volume, isLoop, pos));

        public static void Stop(AudioResource resource) => Core.Stop(resource);
        public static void StopAll() => Core.StopAll();

        public static UniTask FadeOutAsync(AudioSetting setting, float duration = 1) =>
            Core.FadeOutAsync(setting.Resource, duration);

        public static UniTask FadeOutAsync(AudioPlayer player, float duration = 1) =>
            Core.FadeOutAsync(player, duration);

        public static UniTask FadeOutAsync(AudioResource resource, float duration = 1) =>
            Core.FadeOutAsync(resource, duration);

        public static UniTask FadeInAsync(AudioSetting setting, float duration = 1) =>
            Core.FadeInAsync(setting, duration);

        public static UniTask CrossFadeAsync(AudioPlayer prev, AudioSetting setting, float duration = 1)
            => Core.CrossFadeAsync(prev, setting, duration);
    }
}