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
    /// 登録(IScopeRegisterable)・開始(IScopeLaunchable)はどちらも任意で実装する。
    /// 依存の解決はVContainer標準の[Inject]を使えばよく、IScopeMemberとして見つかった時点で
    /// AbstractScopeRoot側が自動的にInjectするため、そのためだけに登録する必要はない。
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
    /// 一度だけ呼び出される処理。このタイミングでは他のすべてのコンポーネントが
    /// (VContainer標準の[Inject]による)依存の解決を終えているため、
    /// 購読の開始など実際に処理を開始する内容はここで行う。
    /// </summary>
    public interface IScopeLaunchable
    {
        void OnLaunch();
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
        /// 自身の子孫すべての登録([Inject]による依存解決を含む)を終えたあと、
        /// ツリー全体で見つかったコンポーネントのOnLaunchをすべて呼び出す。
        /// </summary>
        void Build(IObjectResolver resolver);

        /// <summary>
        /// 自身の子スコープを構築し、見つかったスコープメンバーと、それぞれが属するresolverの組を
        /// すべてcollectorに積み上げる。ここではOnLaunchを呼び出さない
        /// (ツリー全体の登録が終わった最上位ルートだけがまとめて呼び出す)。
        /// </summary>
        void ResolveChildren(
            IObjectResolver resolver,
            List<(object Target, IObjectResolver Resolver)> collector);
    }

    public abstract class AbstractScopeRoot<T> : MonoBehaviour, IScopeRoot
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

            // IScopeMemberとして見つかった全員に対して、登録([IScopeRegisterable])の有無に関わらず
            // [Inject]による依存解決を行う。IScopeRegisterable実装済みの対象は、RegisterComponentの
            // 強制Resolveで既に注入済みだが、Injectは何度呼んでも副作用がないため区別せず一律で呼ぶ。
            foreach (var target in targets)
            {
                SafeInvoke(target, () => Container.Inject(target));
            }

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

            // ツリー全体の登録・[Inject]による依存解決が終わったコンポーネントに対して、
            // OnLaunchをすべて呼び出す。
            foreach (var (target, _) in collector)
            {
                if (target is IScopeLaunchable startable)
                {
                    SafeInvoke(target, startable.OnLaunch);
                }
            }
        }
    }
}