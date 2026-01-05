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

        public static AudioPlayer Play(AudioSetting setting, Transform trackingTarget = null) =>
            Core.Play(setting, trackingTarget);

        public static AudioPlayer Play(AudioClip clip, float volume = 1f, bool isLoop = false,
            Transform trackingTarget = null)
            => Play(new AudioSetting { Clip = clip, Volume = volume, IsLoop = isLoop }, trackingTarget);

        public static bool HasPlay(AudioSetting setting) => Core.HasPlay(setting);

        public static AudioPlayer Ready(AudioSetting setting, Transform trackingTarget = null) =>
            Core.Ready(setting, trackingTarget);

        public static AudioPlayer Ready(AudioClip clip, float volume = 1f, bool isLoop = false,
            Transform trackingTarget = null)
            => Ready(new AudioSetting { Clip = clip, Volume = volume, IsLoop = isLoop }, trackingTarget);

        public static void Stop(AudioClip clip) => Core.Stop(clip);
        public static void StopAll() => Core.StopAll();

        public static UniTask FadeOutAsync(AudioSetting setting, float duration = 1) =>
            Core.FadeOutAsync(setting.Clip, duration);

        public static UniTask FadeOutAsync(AudioPlayer player, float duration = 1) =>
            Core.FadeOutAsync(player, duration);

        public static UniTask FadeOutAsync(AudioClip clip, float duration = 1) =>
            Core.FadeOutAsync(clip, duration);

        public static UniTask FadeInAsync(AudioSetting setting, float duration = 1) =>
            Core.FadeInAsync(setting, duration);

        public static UniTask CrossFadeAsync(AudioPlayer prev, AudioSetting setting, float duration = 1,
            Transform trackingTarget = null)
            => Core.CrossFadeAsync(prev, setting, duration, trackingTarget);
    }
}