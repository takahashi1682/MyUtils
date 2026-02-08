using UnityEngine;

namespace MyUtils.AutoLauncher
{
    /// <summary>
    /// Addressablesで特定のラベルが付与されたPrefabを起動時に自動生成する
    /// </summary>
    public static class AutoLauncher
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            var setting = Resources.Load<AutoLauncherSettings>(nameof(AutoLauncherSettings));
            Debug.Log(setting);
            if (setting == null) return;

            foreach (var obj in setting.LaunchObjects)
            {
                Debug.Log(obj.name);
                if (obj == null) continue;
                Object.Instantiate(obj);
            }

            Resources.UnloadUnusedAssets();
        }
    }
}