using UnityEngine;

namespace MyUtils
{
    /// <summary>
    /// シングルトン基底クラス
    /// </summary>
    /// <typeparam name="T">MonoBehaviourを継承したクラス</typeparam>
    public abstract class AbstractSingletonBehaviour<T> : MonoBehaviour where T : AbstractSingletonBehaviour<T>
    {
        private static T _instance;

        public static T Singleton
        {
            get
            {
                if (_instance == null)
                {
                    // シーン上からインスタンスを探す
                    _instance = FindFirstObjectByType<T>();

                    if (_instance == null)
                    {
                        Debug.LogError($"シーン上に {typeof(T).Name} が見つかりません。");
                    }
                }

                return _instance;
            }
        }

        protected virtual void Awake()
        {
            if (_instance == null) _instance = this as T;

            if (this != _instance)
            {
                Debug.LogError($"シーン上に {nameof(T)} が複数存在します。");
            }
        }

        protected virtual void OnDestroy()
        {
            if (this == _instance) _instance = null;
        }
    }
}