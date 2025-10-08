using System;
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

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    // シーン上からインスタンスを探す
                    _instance = FindFirstObjectByType<T>();

                    if (_instance == null)
                    {
                        throw new Exception($"シーン上に {typeof(T).Name} が見つかりません。");
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
                throw new Exception($"シーン上に {nameof(T)} が複数存在します。");
            }
        }

        protected virtual void OnDestroy()
        {
            if (this == _instance) _instance = null;
        }
    }
}