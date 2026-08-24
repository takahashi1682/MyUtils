using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace MyUtils.VContainerExtensions
{
    public interface IScopeInitializable
    {
        /// <summary>
        /// 依存性注入の登録時に呼び出される初期化処理
        /// </summary>
        /// <param name="builder"></param>
        void OnRegister(IContainerBuilder builder);

        /// <summary>
        /// 依存性注入の解決時に呼び出される初期化処理
        /// </summary>
        /// <param name="resolver"></param>
        void OnResolve(IObjectResolver resolver);
    }

    public abstract class AbstractScopeRoot<T> : MonoBehaviour, IScopeInitializable
        where T : IScopeInitializable
    {
        [Tooltip("Tの検索対象に追加するGameObject(自身の子孫は常に検索対象に含まれます)")]
        [SerializeField] private List<GameObject> _additionalScanRoots = new();

        public IObjectResolver Container { get; protected set; }

        public abstract void OnRegister(IContainerBuilder builder);

        public virtual void OnResolve(IObjectResolver resolver)
        {
            BuildChildScope(resolver);
        }

        protected virtual void RegisterScope(IContainerBuilder builder) { }
        protected virtual void ResolveScope(IObjectResolver resolver) { }

        public void BuildChildScope(IObjectResolver resolver)
        {
            var targets = new List<T>(GetComponentsInChildren<T>(true));

            foreach (var scanRoot in _additionalScanRoots)
            {
                if (scanRoot == null) continue;
                targets.AddRange(scanRoot.GetComponentsInChildren<T>(true));
            }

            Container = resolver.CreateScope(newBuilder =>
            {
                RegisterScope(newBuilder);

                foreach (var target in targets)
                {
                    target.OnRegister(newBuilder);
                }
            });

            foreach (var target in targets)
            {
                target.OnResolve(Container);
            }

            ResolveScope(Container);
        }
    }
}