using UnityEngine;

namespace MyUtils
{
    public class AbstractTargetBehaviour<T> : MonoBehaviour where T : Component
    {
        [Header("対象コンポーネント(未設定時は同一GameObjectのコンポーネントを自動取得)")]
        [SerializeField] protected T _target;

        protected virtual void Awake()
        {
            if (!TryGetComponent(out _target))
            {
                Debug.LogError(
                    $"{gameObject.name} に {typeof(T).Name} コンポーネントがアタッチされていません。");
            }
        }
    }
}