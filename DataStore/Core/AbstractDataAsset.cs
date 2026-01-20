using UnityEngine;

namespace MyUtils.DataStore.Core
{
    public abstract class AbstractDataAsset<T> : ScriptableObject
    {
        public T Data;
    }
}