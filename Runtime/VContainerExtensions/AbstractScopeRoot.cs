using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace MyUtils.VContainerExtensions
{
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
    /// GameObjectの親子関係だけを手がかりに、配下のIScopeRegisterable / IScopeLaunchableを実装した
    /// コンポーネントを収集してスコープを構築する。マーカーや型引数は不要。
    /// 配下に別のAbstractScopeRootがある場合、そのGameObject以下はそちらの管轄として再帰的に任せる。
    /// 収集した対象には[Inject]による依存解決が自動で行われる。
    /// </summary>
    public abstract class AbstractScopeRoot : MonoBehaviour, IScopeRegisterable
    {
        [ReadOnly, SerializeField] private bool _isBuilt;

        [Tooltip("非表示のGameObjectも検索対象に含めるかどうか")]
        [SerializeField] private bool _includeInactive;

        [Tooltip("自身のコンポーネントが無効化されている場合も検索対象に含めるかどうか")]
        [SerializeField] private bool _includeDisableComponent;

        [Tooltip("検索対象に追加するGameObject(自身の子孫は常に検索対象に含まれます)")]
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
        /// こちらは配下すべてに共有される依存(このスコープ固有のシングルトンなど)を登録する。
        /// </summary>
        /// <param name="builder">構築中の子コンテナのbuilder</param>
        protected virtual void ConfigureScope(IContainerBuilder builder) { }

        /// <summary>
        /// rootから親子関係を辿り、スコープの対象となるコンポーネントをtargetsに積む。
        /// 別のAbstractScopeRootに到達したら、そのGameObject以下には降りずにそのルートだけを対象とする。
        /// </summary>
        private void CollectTargets(Transform root, List<object> targets)
        {
            var components = new List<MonoBehaviour>();
            root.GetComponents(components);

            foreach (var component in components)
            {
                if (component == null || ReferenceEquals(component, this)) continue;
                if (!_includeDisableComponent && !component.enabled) continue;

                if (component is AbstractScopeRoot)
                {
                    targets.Add(component);
                    // 入れ子のルートは自身の配下を管理するので、ここでは降りない
                    return;
                }

                if (component is IScopeRegisterable || component is IScopeLaunchable)
                {
                    targets.Add(component);
                }
            }

            foreach (Transform child in root)
            {
                if (!_includeInactive && !child.gameObject.activeSelf) continue;
                CollectTargets(child, targets);
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

        /// <summary>
        /// 自身の子スコープを構築し、見つかったスコープメンバーと、それぞれが属するresolverの組を
        /// すべてcollectorに積み上げる。ここではOnLaunchを呼び出さない
        /// (ツリー全体の登録が終わった最上位ルートだけがまとめて呼び出す)。
        /// </summary>
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

            var targets = new List<object>();
            CollectTargets(transform, targets);

            foreach (var scanRoot in _additionalScanRoots)
            {
                if (scanRoot == null) continue;
                CollectTargets(scanRoot.transform, targets);
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

            // 収集した全員に対して、登録([IScopeRegisterable])の有無に関わらず
            // [Inject]による依存解決を行う。IScopeRegisterable実装済みの対象は、RegisterComponentの
            // 強制Resolveで既に注入済みだが、Injectは何度呼んでも副作用がないため区別せず一律で呼ぶ。
            foreach (var target in targets)
            {
                SafeInvoke(target, () => Container.Inject(target));
            }

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

        /// <summary>
        /// ツリー最上位のルートとして呼び出すエントリーポイント(SceneLifetimeScopeや、
        /// PlayerなどのインスタンスをRuntimeで生成する側から使用)。
        /// 自身の子孫すべての登録([Inject]による依存解決を含む)を終えたあと、
        /// ツリー全体で見つかったコンポーネントのOnLaunchをすべて呼び出す。
        /// </summary>
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
