using System;
using System.Collections.Generic;
using UnityEngine;

namespace MyUtils.FeatureContainer
{
    [Serializable]
    public class FeatureContainer
    {
        private readonly Dictionary<Type, object> _map;

        public FeatureContainer(FeatureContainer source = null)
        {
            _map = source?._map != null
                ? new Dictionary<Type, object>(source._map)
                : new Dictionary<Type, object>();
        }

        /// <summary>
        /// 実行時の型(GetType)で厳格に登録。重複時はエラー。
        /// </summary>
        public void Add<T>(T feature)
        {
            if (feature == null) throw new ArgumentNullException(nameof(feature));

            if (!TryAdd(feature))
            {
                Debug.LogError($"[FeatureContainer] 重複登録(実型): {feature.GetType().Name}");
            }
        }

        /// <summary>
        /// インスタンスの実行時の型(GetType)を使用して登録。
        /// </summary>
        public bool TryAdd(object feature)
        {
            if (feature == null) return false;
            return _map.TryAdd(feature.GetType(), feature);
        }
        
        /// <summary>
        /// 指定した型 T の登録を削除。
        /// </summary>
        public void Remove<T>() where T : class => _map.Remove(typeof(T));

        /// <summary>
        /// 実行時の型(GetType)を使用して上書き登録。
        /// </summary>
        public void Overwrite(object feature)
        {
            if (feature == null) return;
            _map[feature.GetType()] = feature;
        }

        /// <summary>
        /// 厳密な型一致(Key検索)で取得。
        /// </summary>
        public bool TryGet<T>(out T feature) where T : class
        {
            if (_map.TryGetValue(typeof(T), out var val))
            {
                feature = (T)val;
                return true;
            }

            feature = null;
            return false;
        }

        /// <summary>
        /// まず Key 検索を行い、見つからなければ継承関係(is 演算子)をスキャンして取得。
        /// </summary>
        public T Get<T>() where T : class
        {
            if (_map.TryGetValue(typeof(T), out var val))
            {
                return (T)val;
            }

            // 2. 継承・インターフェース関係を走査 (GetType で登録されている場合への対策)
            foreach (var item in _map.Values)
            {
                if (item is T target) return target;
            }

            Debug.LogWarning($"[FeatureContainer] {typeof(T).Name} に適合するインスタンスが見つかりません。");
            return null;
        }

        public void DebugPrint()
        {
            foreach (var kvp in _map)
            {
                Debug.Log($"[FeatureContainer] Key: {kvp.Key.FullName}, Value: {kvp.Value}");
            }
        }
    }
}