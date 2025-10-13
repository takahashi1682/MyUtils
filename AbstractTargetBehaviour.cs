using UnityEngine;

namespace MyUtils
{
    public class AbstractTargetBehaviour<T> : MonoBehaviour where T : Component
    {
        [Header("対象コンポーネント(未設定時は同一GameObjectのコンポーネントを自動取得)")]
        public T Target;

        protected virtual void Awake()
        {
            if (Target == null && !TryGetComponent(out Target))
            {
                Debug.LogError(
                    $"{gameObject.name} に {typeof(T).Name} コンポーネントがアタッチされていません。");
            }
        }
    }
}