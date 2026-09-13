using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VContainer;

namespace MyUtils.VContainerExtensions
{
    /// <summary>
    /// スコープに属するコンポーネントであることを示すだけの空のマーカー。
    /// AbstractScopeRoot&lt;T&gt;がGetComponentsInChildrenで収集する対象を表す。
    /// 実際の初期化処理は、必要なものだけ IScopeRegisterable / IScopeResolvable / IScopeInitiatable を
    /// 個別に実装する(3つすべてを実装する必要はない)。
    /// </summary>
    public interface IScopeMember
    {
    }

    /// <summary>
    /// 依存性注入の登録時に呼び出される初期化処理。
    /// </summary>
    public interface IScopeRegisterable
    {
        void OnRegister(IContainerBuilder builder);
    }

    /// <summary>
    /// スコープツリー全体(自身の親子関係にあるすべてのスコープ)の登録がすべて完了した後、
    /// 一度だけ呼び出される処理。他の場所で登録された依存先も安全にResolveできるタイミング。
    /// ただし他のコンポーネントのOnResolveが完了している保証はないため、ここでは依存の取得のみを行い、
    /// 購読の開始など実際の処理はIScopeInitiatable.OnInitiateで行う。
    /// </summary>
    public interface IScopeResolvable
    {
        void OnResolve(IObjectResolver resolver);
    }

    /// <summary>
    /// スコープツリー全体のOnResolveがすべて完了した後、一度だけ呼び出される処理。
    /// このタイミングでは他のすべてのコンポーネントが依存の解決を終えているため、
    /// 購読の開始など実際に処理を開始する内容はここで行う。
    /// </summary>
    public interface IScopeStartable
    {
        void OnStart();
    }

    /// <summary>
    /// AbstractScopeRoot&lt;T&gt;の型引数に依存しないインターフェース。
    /// 型引数Tが異なる入れ子のスコープルート同士が、お互いの型を知らなくても再帰的に連携できるようにする。
    /// SceneLifetimeScopeのように型引数を意識したくない側からは、このインターフェースを経由して起動する。
    /// </summary>
    public interface IScopeRoot : IScopeRegisterable
    {
        /// <summary>
        /// ツリー最上位のルートとして呼び出すエントリーポイント(SceneLifetimeScopeや、
        /// PlayerなどのインスタンスをRuntimeで生成する側から使用)。
        /// 自身の子孫すべての登録を終えたあと、ツリー全体で見つかったコンポーネントの
        /// OnResolveをすべて呼び終えてから、OnInitiateをすべて呼び出す。
        /// </summary>
        void Build(IObjectResolver resolver);

        /// <summary>
        /// 自身の子スコープを構築し、見つかったスコープメンバーと、それぞれが属するresolverの組を
        /// すべてcollectorに積み上げる。ここではOnResolve/OnInitiateを呼び出さない
        /// (ツリー全体の登録が終わった最上位ルートだけがまとめて呼び出す)。
        /// </summary>
        void ResolveChildren(
            IObjectResolver resolver,
            List<(object Target, IObjectResolver Resolver)> collector);
    }

    public abstract class AbstractScopeRoot<T> : MonoBehaviour, IScopeRoot, IScopeResolvable, IScopeStartable
        where T : IScopeMember
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

        /// <inheritdoc/>
        public virtual void OnStart() { }

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

        /// <summary>
        /// IScopeRegisterable/IScopeResolvable/IScopeInitiatableのどれも実装していないスキャン対象は、
        /// マーカー(T)だけ実装して中身を実装し忘れた凡ミスの可能性が高いため警告する。
        /// </summary>
        private static void WarnIfNoPhaseImplemented(IEnumerable<T> targets)
        {
            foreach (var target in targets)
            {
                if (target is IScopeRegisterable or IScopeResolvable or IScopeStartable) continue;

                var unityObject = target as UnityEngine.Object;
                Debug.LogWarning(
                    $"{unityObject?.name ?? target.GetType().Name}: " +
                    $"{typeof(T).Name}を実装していますが、IScopeRegisterable/IScopeResolvable/IScopeInitiatableの" +
                    "いずれも実装していないため何も呼び出されません。実装し忘れではありませんか?",
                    unityObject);
            }
        }

        /// <summary>
        /// actionの実行中に例外が発生しても、どのオブジェクトが原因かログに残したうえで
        /// 処理を継続する(ツリー全体の初期化が1箇所の例外で丸ごと止まるのを防ぐ)。
        /// </summary>
        private static void SafeInvoke(object target, Action action)
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                var unityObject = target as UnityEngine.Object;
                Debug.LogException(e, unityObject);
            }
        }

        /// <inheritdoc/>
        public void ResolveChildren(
            IObjectResolver resolver,
            List<(object Target, IObjectResolver Resolver)> collector)
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

            WarnIfNoPhaseImplemented(targets);

            Container = resolver.CreateScope(newBuilder =>
            {
                ConfigureScope(newBuilder);

                foreach (var target in targets)
                {
                    if (target is IScopeRegisterable registerable)
                    {
                        SafeInvoke(target, () => registerable.OnRegister(newBuilder));
                    }
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
            var collector = new List<(object Target, IObjectResolver Resolver)>();
            ResolveChildren(resolver, collector);

            // ツリー全体の登録が終わったコンポーネントに対して、まずOnResolveをすべて呼び終える。
            foreach (var (target, targetResolver) in collector)
            {
                if (target is IScopeResolvable resolvable)
                {
                    SafeInvoke(target, () => resolvable.OnResolve(targetResolver));
                }
            }

            // 他のすべてのコンポーネントが依存解決を終えた後に、OnInitiateをすべて呼び出す。
            foreach (var (target, _) in collector)
            {
                if (target is IScopeStartable initiatable)
                {
                    SafeInvoke(target, initiatable.OnStart);
                }
            }
        }
    }
}
