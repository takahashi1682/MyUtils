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
        public static T Instance { get; private set; }
        private static UniTaskCompletionSource<T> _source = new();
        public static UniTask<T> WaitInstanceAsync => _source.Task;

        protected virtual void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Debug.LogError($"{this} は既に存在しています。重複したインスタンスを破棄します。");
                Destroy(gameObject);
                return;
            }

            Instance = this as T;
            _source.TrySetResult(Instance);
        }

        protected virtual void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
                _source = new UniTaskCompletionSource<T>();
            }
        }
    }
}