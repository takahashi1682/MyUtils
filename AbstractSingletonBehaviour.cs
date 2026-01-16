using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MyUtils
{
    /// <summary>
    /// シングルトン基底クラス
    /// </summary>
    /// <typeparam name="T">MonoBehaviourを継承したクラス</typeparam>
    public abstract class AbstractSingletonBehaviour<T> : MonoBehaviour where T : AbstractSingletonBehaviour<T>
    {
        public static T Instance { get; protected set; }
        protected static UniTaskCompletionSource<T> Source = new();
        public static UniTask<T> AsyncInstance => Source.Task;

        protected virtual void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Debug.LogError($"{this} は既に存在しています。重複したインスタンスを破棄します。");
                Destroy(gameObject);
                return;
            }

            Instance = this as T;
            Source.TrySetResult(Instance);
        }

        protected virtual void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
                Source = new UniTaskCompletionSource<T>();
            }
        }
    }
}