using UnityEngine;
using UnityEngine.AddressableAssets;

namespace MyUtils.AutoLauncher
{
    /// <summary>
    /// Addressablesで特定のラベルが付与されたPrefabを起動時に自動生成する
    /// </summary>
    public static class AutoLauncher
    {
        // Addressables側で設定するラベル名
        private const string LaunchLabel = "AutoLaunch";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static async void Initialize()
        {
            // ラベルがついた全てのアセットを非同期ロード
            var handle = Addressables.LoadAssetsAsync<GameObject>(LaunchLabel, null);

            try
            {
                var prefabs = await handle.Task;

                if (prefabs == null || prefabs.Count == 0) return;

                foreach (var prefab in prefabs)
                {
                    if (prefab != null)
                    {
                        Object.Instantiate(prefab);
                    }
                }
            }
            finally
            {
                // ロード用ハンドルの解放（生成したインスタンスは残る）
                Addressables.Release(handle);
            }
        }
    }
}