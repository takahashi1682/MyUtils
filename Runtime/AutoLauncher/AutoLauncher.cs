using System.Linq;
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
            var settings = Resources.LoadAll<AutoLauncherSettings>(string.Empty);
            if (settings == null || settings.Length == 0) return;

            var targets = settings.SelectMany(s => s.LaunchObjects).Where(obj => obj != null).Distinct();

            foreach (var obj in targets)
            {
                Object.Instantiate(obj);
            }
        }
    }
}