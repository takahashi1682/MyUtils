using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MyUtils
{
    /// <summary>
    /// 複合キー：オブジェクト名 + コンポーネント型名
    /// </summary>
    public class ComponentKey : IEquatable<ComponentKey>
    {
        public readonly string ObjectName;
        public readonly string ComponentType;

        public ComponentKey(string objectName, string componentType)
        {
            ObjectName = objectName ?? string.Empty;
            ComponentType = componentType ?? string.Empty;
        }

        public bool Equals(ComponentKey other) =>
            other != null && ObjectName == other.ObjectName && ComponentType == other.ComponentType;

        public override bool Equals(object obj) =>
            obj is ComponentKey other && Equals(other);

        public override int GetHashCode() =>
            HashCode.Combine(ObjectName, ComponentType);
    }

    /// <summary>
    /// シーン上のオブジェクトおよびそのコンポーネントの取得を管理するシングルトンクラス
    /// </summary>
    public class SceneObjectsRegistry : MonoBehaviour
    {
        [SerializeField] private GameObject[] _registeredObjects;

        private readonly Dictionary<ComponentKey, Component> _componentCache = new();
        private static SceneObjectsRegistry _instance;

        public static SceneObjectsRegistry Singleton
        {
            get
            {
                if (_instance == null)
                {
                    // シーン上からインスタンスを探す
                    _instance = FindFirstObjectByType<SceneObjectsRegistry>();

                    if (_instance == null)
                    {
                        throw new Exception($"シーン上に {nameof(SceneObjectsRegistry)} が見つかりません。");
                    }
                }

                return _instance;
            }
        }

        private void Awake()
        {
            if (_instance == null) _instance = this;

            if (this != _instance)
            {
                throw new Exception($"シーン上に {nameof(SceneObjectsRegistry)} が複数存在します。");
            }
        }

        protected void OnDestroy()
        {
            if (this == _instance) _instance = null;
        }

        /// <summary>
        /// 登録された GameObject を名前で取得します
        /// </summary>
        public static GameObject FindObjectByName(string objectName)
        {
            if (Singleton == null || string.IsNullOrEmpty(objectName)) return null;
            return Singleton._registeredObjects.FirstOrDefault(obj => obj.name == objectName);
        }

        /// <summary>
        /// 登録された GameObject から指定の型のコンポーネントを取得します（キャッシュ使用）
        /// </summary>
        public static T GetComponentFromScene<T>(string objectName = null) where T : Component
        {
            if (Singleton == null) return null;

            string typeName = typeof(T).Name;
            var componentKey = new ComponentKey(objectName ?? string.Empty, typeName);

            if (Singleton._componentCache.TryGetValue(componentKey, out var cachedComponent))
                return cachedComponent as T;

            foreach (var obj in Singleton._registeredObjects)
            {
                if (!string.IsNullOrEmpty(objectName) && obj.name != objectName) continue;

                if (obj.TryGetComponent<T>(out var component))
                {
                    Singleton._componentCache[componentKey] = component;
                    return component;
                }
            }

            return null;
        }

        /// <summary>
        /// Tryパターン：Component を安全に取得します
        /// </summary>
        public static bool TryGetComponentFromScene<T>(out T component, string objectName = null) where T : Component
        {
            component = GetComponentFromScene<T>(objectName);
            return component != null;
        }

        /// <summary>
        /// キャッシュをクリア（リロード時や一時的に再スキャンしたい場合に使用）
        /// </summary>
        public static void ClearCache() => Singleton._componentCache.Clear();

        /// <summary>
        /// 任意の GameObject を追加登録（動的生成時など）
        /// </summary>
        public static void RegisterObject(GameObject obj)
        {
            if (Singleton == null || obj == null) return;

            var list = Singleton._registeredObjects.ToList();
            if (!list.Contains(obj))
            {
                list.Add(obj);
                Singleton._registeredObjects = list.ToArray();
            }
        }
    }
}