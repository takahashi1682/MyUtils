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
            _map = source == null
                ? new Dictionary<Type, object>()
                : new Dictionary<Type, object>(source._map);
        }

        /// <summary>
        /// 厳格な登録。既に同じ型が登録されている場合はエラーログを出し、上書きしません。
        /// </summary>
        public void Add<T>(T feature) where T : class
        {
            if (feature == null) throw new ArgumentNullException(nameof(feature));

            if (!TryAdd(feature))
            {
                Debug.LogError($"[FeatureContainer] 重複した登録を検知しました。型: {typeof(T).Name}");
            }
        }

        /// <summary>
        /// 柔軟な登録。登録に成功した場合は true、既に存在した場合は false を返します。
        /// </summary>
        public bool TryAdd<T>(T feature) where T : class
        {
            if (feature == null) return false;
            return _map.TryAdd(typeof(T), feature);
        }

        public void Remove<T>() where T : class
        {
            _map.Remove(typeof(T));
        }

        public void Overwrite<T>(T feature) where T : class
        {
            if (feature == null) return;
            _map[typeof(T)] = feature;
        }

        /// <summary>
        /// 厳密な型一致で取得
        /// </summary>
        /// <param name="feature"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
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
        /// 厳密な型一致で取得
        /// </summary>
        public T Get<T>() where T : class
        {
            if (TryGet<T>(out var feature)) return feature;

            Debug.LogWarning($"Feature {typeof(T).Name} が登録されていません。");
            return null;
        }
    }
}