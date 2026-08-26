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
        /// スコープツリー全体（自身の親子関係にあるすべてのスコープ）の登録・解決がすべて完了した後、
        /// 一度だけ呼び出される処理。他の場所で解決された依存先も安全に参照できるタイミング。
        /// </summary>
        /// <param name="resolver">自身が登録されているスコープのresolver</param>
        void OnResolve(IObjectResolver resolver);
    }

    /// <summary>
    /// AbstractScopeRoot&lt;T&gt;の非ジェネリックな基底クラス。
    /// 型引数Tが異なる入れ子のスコープルート同士が、お互いの型を知らなくても再帰的に連携できるようにする。
    /// SceneLifetimeScopeのように型引数を意識したくない側からは、このクラスを経由して起動する。
    /// </summary>
    public abstract class AbstractScopeRoot : MonoBehaviour, IScopeInitializable
    {
        public virtual void OnRegister(IContainerBuilder builder) { }
        public virtual void OnResolve(IObjectResolver resolver) { }

        /// <summary>
        /// ツリー最上位のルートとして呼び出すエントリーポイント（SceneLifetimeScopeから使用）。
        /// 自身の子孫すべての登録を終えたあと、ツリー全体で見つかったIScopeInitializableに対して
        /// 一度だけOnResolveを呼び出す。
        /// </summary>
        public void Run(IObjectResolver resolver)
        {
            var collector = new List<(IScopeInitializable Target, IObjectResolver Resolver)>();
            ResolveChildren(resolver, collector);

            foreach (var (target, targetResolver) in collector)
            {
                target.OnResolve(targetResolver);
            }
        }

        /// <summary>
        /// 自身の子スコープを構築し、見つかったIScopeInitializableと、それぞれが属するresolverの組を
        /// すべてcollectorに積み上げる。ここではOnResolveを呼び出さない
        /// （ツリー全体の登録が終わった最上位ルートだけがまとめて呼び出す）。
        /// </summary>
        internal abstract void ResolveChildren(
            IObjectResolver resolver,
            List<(IScopeInitializable Target, IObjectResolver Resolver)> collector);
    }

    public abstract class AbstractScopeRoot<T> : AbstractScopeRoot where T : IScopeInitializable
    {
        [Tooltip("Tの検索対象に追加するGameObject(自身の子孫は常に検索対象に含まれます)")]
        [SerializeField] private List<GameObject> _additionalScanRoots = new();

        public IObjectResolver Container { get; protected set; }

        [ReadOnly] private bool _isBuilt;

        protected virtual void ConfigureScope(IContainerBuilder builder) { }

        internal override void ResolveChildren(
            IObjectResolver resolver,
            List<(IScopeInitializable Target, IObjectResolver Resolver)> collector)
        {
            if (_isBuilt)
            {
                Debug.LogWarning($"{name}: scope was already built.", this);
                return;
            }
            _isBuilt = true;

            var targets = new List<T>(GetComponentsInChildren<T>(true));

            foreach (var scanRoot in _additionalScanRoots)
            {
                if (scanRoot == null) continue;
                targets.AddRange(scanRoot.GetComponentsInChildren<T>(true));
            }

            Container = resolver.CreateScope(newBuilder =>
            {
                ConfigureScope(newBuilder);

                foreach (var target in targets)
                {
                    target.OnRegister(newBuilder);
                }
            });

            collector.Add((this, Container));

            foreach (var target in targets)
            {
                if (target is AbstractScopeRoot nestedRoot)
                {
                    nestedRoot.ResolveChildren(Container, collector);
                }
                else
                {
                    collector.Add((target, Container));
                }
            }
        }
    }
}