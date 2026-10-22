using UnityEngine;

namespace MyUtils.Abstract
{
    /// <summary>
    /// 同じGameObjectの T コンポーネントを対象(Target)として扱う基底クラス。
    /// Targetは Awake で GetComponent して設定する(Inspectorからは設定できない)。
    /// 派生クラスには [RequireComponent(typeof(T))] を付けること
    /// (ジェネリックの基底クラスには、型パラメーターを使った属性を書けないため)。
    /// </summary>
    public abstract class AbstractTargetBehaviour<T> : MonoBehaviour where T : Component
    {
        /// <summary>同じGameObjectにある対象コンポーネント</summary>
        public T Target { get; private set; }

        protected virtual void Awake()
        {
            Target = GetComponent<T>();

            if (Target == null)
            {
                Debug.LogError($"{gameObject.name} に {typeof(T).Name} コンポーネントがアタッチされていません。", this);
            }
        }

        // 互換用。派生クラスの base.Start() を、そのまま呼べるようにする
        protected virtual void Start() { }
    }
}
