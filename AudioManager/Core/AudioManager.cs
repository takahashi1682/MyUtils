using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.Audio;

namespace MyUtils.AudioManager.Core
{
    [Serializable]
    public class AudioManager
    {
        public ReactiveProperty<float> VolumeRate;
        public AudioMixerGroup MixerGroup;

        private readonly MonoBehaviour _parent;
        private readonly List<AudioPlayer> _audioPlayers = new();
        private CancellationTokenSource _cts;

        public AudioManager(
            MonoBehaviour parent,
            ReactiveProperty<float> volumeRate,
            AudioMixerGroup mixerGroup,
            int maxAudioStreams)
        {
            _parent = parent;
            VolumeRate = volumeRate;
            MixerGroup = mixerGroup;

            for (int i = 0; i < maxAudioStreams; i++)
                CreateAudioPlayer($"{MixerGroup.name}_{i}");
        }

        ~AudioManager() => _cts?.Cancel();

        public AudioPlayer Ready(AudioSetting setting)
        {
            if (!TryGetAvailablePlayer(out var player)) return null;
            player.Ready(setting);
            return player;
        }

        public AudioPlayer Play(AudioSetting setting)
        {
            var player = Ready(setting);
            player?.Play();
            return player;
        }

        public async void PlayFadeAsync(AudioSetting setting, float duration = 1)
        {
            var player = Play(setting);
            if (player == null) return;
            await FadeInAsync(player, duration);
        }

        public AudioPlayer HasPlay(AudioSetting setting)
        {
            var player = GetPlayingAudioPlayer(setting.Resource);
            return player ?? Play(setting);
        }

        public AudioPlayer GetPlayingAudioPlayer(AudioResource resource)
            => _audioPlayers.FirstOrDefault(p =>
                p.IsInUse &&
                p.AudioSource?.clip?.name == resource.name);

        public void Stop(AudioResource resource)
            => _audioPlayers.FirstOrDefault(p =>
                p.AudioSource?.clip?.name == resource.name)?.Stop();

        public void StopAll()
            => _audioPlayers.ForEach(p => p.Stop());

        protected void CreateAudioPlayer(string name)
        {
            var go = new GameObject(name);
            go.transform.SetParent(_parent.transform);
            var player = go.AddComponent<AudioPlayer>();
            player.Initialize(this);
            _audioPlayers.Add(player);
        }

        protected bool TryGetAvailablePlayer(out AudioPlayer player)
        {
            player = _audioPlayers.FirstOrDefault(p => !p.IsInUse);
            return player != null;
        }

        protected List<AudioPlayer> GetPlayingAudioPlayers()
            => _audioPlayers.Where(p => p.IsInUse).ToList();

        protected bool IsPlaying() => GetPlayingAudioPlayers().Count > 0;

        protected CancellationToken ResetCancellationToken()
        {
            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            return _cts.Token;
        }

        public async UniTask FadeOutAsync(AudioResource resource, float duration = 1)
        {
            var player = GetPlayingAudioPlayer(resource);
            if (player == null) return;
            await FadeOutAsync(player, duration);
            player.Stop();
        }

        public async UniTask FadeOutAsync(AudioPlayer player, float duration = 1)
        {
            await UniTaskUtils.LerpAsync(player.VolumeRate.CurrentValue, 0, duration, x => player.VolumeRate.Value = x, token: player.Cts.Token);
        }

        public async UniTask FadeInAsync(AudioSetting setting, float duration = 1)
        {
            var player = Play(setting);
            if (player == null) return;
            await FadeInAsync(player, duration);
        }

        public async UniTask FadeInAsync(AudioPlayer player, float duration = 1)
            => await UniTaskUtils.LerpAsync(0, player.VolumeRate.CurrentValue, duration, x => player.VolumeRate.Value = x, token: player.Cts.Token);

        public async UniTask CrossFadeAsync(AudioPlayer prev, AudioSetting setting, float duration)
        {
            await UniTaskUtils.LerpAsync(prev.VolumeRate.CurrentValue, 0, duration, x => prev.VolumeRate.Value = x, token: prev.Cts.Token);
            prev.Stop();
            var player = Play(setting);
            await UniTaskUtils.LerpAsync(0,  setting.Volume, duration, x => player.VolumeRate.Value = x, token: player.Cts.Token);
        }
    }
}