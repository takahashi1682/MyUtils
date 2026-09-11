using System.Collections.Generic;
using System.Linq;
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
    /// AbstractScopeRoot&lt;T&gt;の型引数に依存しないインターフェース。
    /// 型引数Tが異なる入れ子のスコープルート同士が、お互いの型を知らなくても再帰的に連携できるようにする。
    /// SceneLifetimeScopeのように型引数を意識したくない側からは、このインターフェースを経由して起動する。
    /// </summary>
    public interface IScopeRoot : IScopeInitializable
    {
        /// <summary>
        /// ツリー最上位のルートとして呼び出すエントリーポイント（SceneLifetimeScopeや、
        /// PlayerなどのインスタンスをRuntimeで生成する側から使用）。
        /// 自身の子孫すべての登録を終えたあと、ツリー全体で見つかったIScopeInitializableに対して
        /// 一度だけOnResolveを呼び出す。
        /// </summary>
        void Build(IObjectResolver resolver);

        /// <summary>
        /// 自身の子スコープを構築し、見つかったIScopeInitializableと、それぞれが属するresolverの組を
        /// すべてcollectorに積み上げる。ここではOnResolveを呼び出さない
        /// （ツリー全体の登録が終わった最上位ルートだけがまとめて呼び出す）。
        /// </summary>
        void ResolveChildren(
            IObjectResolver resolver,
            List<(IScopeInitializable Target, IObjectResolver Resolver)> collector);
    }

    public abstract class AbstractScopeRoot<T> : MonoBehaviour, IScopeRoot where T : IScopeInitializable
    {
        [ReadOnly, SerializeField] private bool _isBuilt;

        [Tooltip("非表示のGameObjectも検索対象に含めるかどうか")]
        [SerializeField] private bool _includeInactive;

        [Tooltip("自身のコンポーネントが無効化されている場合も検索対象に含めるかどうか")]
        [SerializeField] private bool _includeDisableComponent;

        [Tooltip("Tの検索対象に追加するGameObject(自身の子孫は常に検索対象に含まれます)")]
        [SerializeField] private List<GameObject> _additionalScanRoots = new();

        /// <summary>
        /// このスコープルートが構築した子コンテナのresolver。
        /// <see cref="ResolveChildren"/>が呼ばれるまでは未構築(null)。
        /// </summary>
        public IObjectResolver Container { get; private set; }

        /// <inheritdoc/>
        public virtual void OnRegister(IContainerBuilder builder) { }

        /// <inheritdoc/>
        public virtual void OnResolve(IObjectResolver resolver) { }

        /// <summary>
        /// このスコープルートが構築する子コンテナ自体への登録処理。
        /// <see cref="OnRegister"/>が親コンテナへの自己登録であるのに対し、
        /// こちらは配下のTすべてに共有される依存(このスコープ固有のシングルトンなど)を登録する。
        /// </summary>
        /// <param name="builder">構築中の子コンテナのbuilder</param>
        protected virtual void ConfigureScope(IContainerBuilder builder) { }

        /// <summary>
        /// _includeDisableComponentがfalseの場合、Behaviour(MonoBehaviourなど)で
        /// enabled=falseになっているものを除外する。Behaviourでない(enabledを持たない)Tはそのまま含める。
        /// </summary>
        private IEnumerable<T> FilterDisabledComponents(IEnumerable<T> components)
        {
            return _includeDisableComponent ?
                components :
                components.Where(c => c is not Behaviour behaviour || behaviour.enabled);
        }

        /// <inheritdoc/>
        public void ResolveChildren(
            IObjectResolver resolver,
            List<(IScopeInitializable Target, IObjectResolver Resolver)> collector)
        {
            if (_isBuilt)
            {
                Debug.LogWarning($"{name}: scope was already built.", this);
                return;
            }

            _isBuilt = true;

            var targets = new List<T>(FilterDisabledComponents(GetComponentsInChildren<T>(_includeInactive)));

            foreach (var scanRoot in _additionalScanRoots)
            {
                if (scanRoot == null) continue;
                targets.AddRange(FilterDisabledComponents(scanRoot.GetComponentsInChildren<T>(_includeInactive)));
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
                if (target is IScopeRoot nestedRoot)
                {
                    nestedRoot.ResolveChildren(Container, collector);
                }
                else
                {
                    collector.Add((target, Container));
                }
            }
        }

        /// <inheritdoc/>
        public void Build(IObjectResolver resolver)
        {
            var collector = new List<(IScopeInitializable Target, IObjectResolver Resolver)>();
            ResolveChildren(resolver, collector);

            foreach (var (target, targetResolver) in collector)
            {
                target.OnResolve(targetResolver);
            }
        }
    }
}