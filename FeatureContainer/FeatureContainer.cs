using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MyUtils.FeatureContainer
{
    public class FeatureContainer
    {
        public Dictionary<Type, object> Map { get; }

        public FeatureContainer(FeatureContainer source = null)
        {
            Map = source == null ? new Dictionary<Type, object>() : new Dictionary<Type, object>(source.Map);
        }

        public void TryAdd(object feature)
        {
            if (feature == null) return;
            var type = feature.GetType();

            if (!Map.TryAdd(type, feature))
            {
                Debug.LogError($"重複したFeature登録を検知: {type.Name}");
            }
        }

        public T Get<T>() where T : class
        {
            if (Map.TryGetValue(typeof(T), out object val)) return (T)val;

            var found = Map.Values.OfType<T>().FirstOrDefault();
            if (found != null) return found;

            Debug.LogWarning($"Feature {typeof(T).Name} が見つかりませんでした。");
            return null;
        }
    }
}