#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using MyUtils.Misc;
using Object = UnityEngine.Object;

namespace MyUtils.DebugMethod
{
    /// <summary>
    /// [DebugMethod] がついたメソッドを集める。
    /// シーンのオブジェクトは再生の開始・終了で作り直されるので static には持たず、型ごとのメソッド一覧だけをキャッシュする。
    /// </summary>
    public static class DebugMethodRegistry
    {
        private const BindingFlags Flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        // 型はスクリプトを変えない限り変わらないので、static に残しても古くならない
        private static readonly Dictionary<Type, MethodInfo[]> _methodsByType = new();

        public static MethodInfo[] GetMethods(Type type)
        {
            if (!_methodsByType.TryGetValue(type, out var methods))
            {
                methods = type.GetMethods(Flags)
                    .Where(m => m.IsDefined(typeof(DebugMethodAttribute), true))
                    .ToArray();
                _methodsByType[type] = methods;
            }

            return methods;
        }

        /// <summary>1つのコンポーネントのデバッグメソッド(インスペクター用。型ごとにキャッシュしているので軽い)</summary>
        public static List<DebugMethodCache> GetMethodsForTarget(MonoBehaviour target)
        {
            return GetMethods(target.GetType()).Select(method => CreateCache(target, method)).ToList();
        }

        /// <summary>開いているシーンの全デバッグメソッド(ウィンドウ用)。全コンポーネントを探すので、必要なときだけ呼ぶ</summary>
        public static List<DebugMethodCache> FindAllInScene()
        {
            return Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include)
                .Where(mono => mono != null)
                .SelectMany(GetMethodsForTarget)
                .ToList();
        }

        private static DebugMethodCache CreateCache(MonoBehaviour mono, MethodInfo method)
        {
            string sceneName = mono.gameObject.scene.name ?? "UntitledScene";
            string paramTypeKey = string.Join("_", method.GetParameters().Select(p => p.ParameterType.Name));
            string methodUniqueName = string.IsNullOrEmpty(paramTypeKey) ? method.Name : $"{method.Name}_{paramTypeKey}";

            return new DebugMethodCache
            {
                Target = mono,
                Info = method,
                SaveKeyBase = $"DebugInvoke.{sceneName}.{mono.gameObject.name}.{methodUniqueName}"
            };
        }
    }
}
#endif
