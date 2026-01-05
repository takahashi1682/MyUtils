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

        protected virtual void Awake()
        {
            // 同じ型(T)のCoreが既に存在するかチェック
            if (Core != null)
            {
                Debug.LogWarning($"[AudioManager] {typeof(T).Name} のインスタンスが既に存在するため、新しい方を破棄します。");
                Destroy(gameObject);
                return;
            }

            // この型(T)専用のCoreを生成
            Core = new AudioManager(this, VolumeRate, MixerGroup, MaxAudioStreams);
        }

        protected virtual void OnDestroy()
        {
            Core = null;
        }

        // ==== Wrappers ====

        public static AudioPlayer Play(AudioSetting setting) => Core.Play(setting);

        public static AudioPlayer Play(AudioResource resource, float volume = 1f, bool isLoop = false)
            => Play(new AudioSetting(resource, volume, isLoop));

        public static bool HasPlay(AudioSetting setting) => Core.HasPlay(setting);

        public static bool HasPlay(AudioResource resource, float volume = 1f, bool isLoop = false)
            => Core.HasPlay(new AudioSetting(resource, volume, isLoop));

        public static AudioPlayer Ready(AudioSetting setting) => Core.Ready(setting);

        public static AudioPlayer Ready(AudioResource resource, float volume = 1f, bool isLoop = false)
            => Ready(new AudioSetting(resource, volume, isLoop));

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