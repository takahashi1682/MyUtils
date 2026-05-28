using UnityEngine;

namespace MyUtils.DataStore
{
    public abstract class AbstractDataAsset<T> : ScriptableObject
    {
        public T Data;

        public T Clone() => JsonUtility.FromJson<T>(JsonUtility.ToJson(Data));
    }
}