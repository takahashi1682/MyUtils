using System;
using UnityEngine;

namespace MyUtils
{
    [Serializable]
    public class SerializableKeyPair<TKey, TValue>
    {
        [field: SerializeField] public TKey Key { get; private set; }
        [field: SerializeField] public TValue Value { get; private set; }

        public SerializableKeyPair(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }
    }
}