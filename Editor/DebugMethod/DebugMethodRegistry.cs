#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace MyUtils.DebugMethod
{
    /// <summary>
    /// シーン内のデバッグ対象メソッドを中央管理するレジストリ
    /// </summary>
    public static class DebugMethodRegistry
    {
        private static readonly List<DebugMethodCache> _globalCache = new();
        public static IReadOnlyList<DebugMethodCache> GlobalCache => _globalCache;

        public static void RefreshCache()
        {
            _globalCache.Clear();
            var allComponents =
                Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include,
                    FindObjectsSortMode.None);

            foreach (var mono in allComponents)
            {
                if (mono == null) continue;

                var methods = mono.GetType()
                    .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where(m => m.GetCustomAttributes(typeof(DebugMethodAttribute), true).Length > 0);

                foreach (var method in methods)
                {
                    string sceneName = mono.gameObject.scene.name ?? "UntitledScene";
                    var parameters = method.GetParameters();
                    string paramTypeKey = string.Join("_", parameters.Select(p => p.ParameterType.Name));
                    string methodUniqueName = string.IsNullOrEmpty(paramTypeKey)
                        ? method.Name
                        : $"{method.Name}_{paramTypeKey}";

                    _globalCache.Add(new DebugMethodCache
                    {
                        Target = mono,
                        Info = method,
                        SaveKeyBase = $"DebugInvoke.{sceneName}.{mono.gameObject.name}.{methodUniqueName}"
                    });
                }
            }
        }

        public static List<DebugMethodCache> GetMethodsForTarget(MonoBehaviour target)
        {
            if (_globalCache.Count == 0) RefreshCache();
            return _globalCache.Where(c => c.Target == target).ToList();
        }
    }
}
#endif