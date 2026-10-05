using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace MyUtils.TalkUtils
{
    /// <summary>
    /// セリフのボイス(AudioClip)を Addressables から読み込む。
    /// 読み込んだクリップはアドレスごとに保持し、同じアドレスは再読み込みしない(解放はしない)
    /// </summary>
    internal sealed class TalkVoiceLoader
    {
        private readonly Dictionary<string, AudioClip> _cache = new();

        /// <summary>
        /// ボイスのアドレスが設定されたセリフに、ボイスを読み込んで設定する
        /// </summary>
        public async UniTask LoadAsync(IReadOnlyList<TalkLine> talk, CancellationToken ct)
        {
            foreach (var line in talk)
            {
                if (line.Voice != null || string.IsNullOrEmpty(line.VoiceAddress)) continue;

                line.Voice = await LoadAsync(line.VoiceAddress, ct);
            }
        }

        // 見つからなければ警告を出してnullを返す(ボイスなしとして会話は続ける)
        private async UniTask<AudioClip> LoadAsync(string address, CancellationToken ct)
        {
            if (_cache.TryGetValue(address, out var cached)) return cached;

            if (!await ExistsAsync(address, ct))
            {
                Debug.LogWarning($"[TalkVoiceLoader] ボイス '{address}' が見つかりません");
                return null;
            }

            var handle = Addressables.LoadAssetAsync<AudioClip>(address);
            var clip = await handle.Task.AsUniTask().AttachExternalCancellation(ct);

            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogWarning($"[TalkVoiceLoader] ボイス '{address}' の読み込みに失敗しました");
                return null;
            }

            _cache[address] = clip;
            return clip;
        }

        private static async UniTask<bool> ExistsAsync(string address, CancellationToken ct)
        {
            var handle = Addressables.LoadResourceLocationsAsync(address, typeof(AudioClip));
            try
            {
                var locations = await handle.Task.AsUniTask().AttachExternalCancellation(ct);
                return locations.Count > 0;
            }
            finally
            {
                Addressables.Release(handle);
            }
        }
    }
}
