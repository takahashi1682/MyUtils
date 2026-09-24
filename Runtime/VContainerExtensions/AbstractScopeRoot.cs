using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VContainer;

namespace MyUtils.VContainerExtensions
{
    /// <summary>登録処理(依存の登録はここで行う)。</summary>
    public interface IScopeRegisterable
    {
        void OnRegister(IContainerBuilder builder);
    }

    /// <summary>全員の登録と[Inject]が終わった後に一度だけ呼ばれる開始処理。</summary>
    public interface IScopeLaunchable
    {
        void OnLaunch();
    }

    /// <summary>自身と子孫のMonoBehaviourを集めてスコープを作り、[Inject]・OnRegister・OnLaunchを行う。</summary>
    public abstract class AbstractScopeRoot : MonoBehaviour, IScopeRegisterable
    {
        [ReadOnly, SerializeField] private bool _isBuilt;

        [Tooltip("非表示のGameObjectも対象に含めるかどうか")]
        [SerializeField] private bool _includeInactive;

        [Tooltip("無効化されたコンポーネントも対象に含めるかどうか")]
        [SerializeField] private bool _includeDisableComponent;

        [Tooltip("子孫以外に対象に加えるGameObject")]
        [SerializeField] private List<GameObject> _additionalScanRoots = new();

        /// <summary>このスコープが作った子コンテナ(構築前はnull)。</summary>
        public IObjectResolver Container { get; private set; }

        /// <summary>親のコンテナへの登録処理。</summary>
        public virtual void OnRegister(IContainerBuilder builder) { }

        /// <summary>このスコープ内で共有する依存の登録処理。</summary>
        protected virtual void ConfigureScope(IContainerBuilder builder) { }

        // 別のAbstractScopeRootに当たったら、そのルートだけを加えて、その先には降りない。
        private void Collect(Transform root, List<MonoBehaviour> members)
        {
            var components = root.GetComponents<MonoBehaviour>()
                .Where(c => c != null && c != this && (_includeDisableComponent || c.enabled))
                .ToList();

            var nestedRoot = components.OfType<AbstractScopeRoot>().FirstOrDefault();
            if (nestedRoot != null)
            {
                members.Add(nestedRoot);
                return;
            }

            members.AddRange(components);

            foreach (Transform child in root)
            {
                if (_includeInactive || child.gameObject.activeSelf) Collect(child, members);
            }
        }

        // 例外が起きても原因をログに出して、残りの処理は続ける。
        private static void SafeInvoke(object target, Action action)
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                Debug.LogException(e, target as UnityEngine.Object);
            }
        }

        // 子スコープを作って全員に注入し、OnLaunchの対象をlaunchTargetsに積む(呼び出しはBuildがまとめて行う)。
        private void ResolveChildren(IObjectResolver resolver, List<MonoBehaviour> launchTargets)
        {
            if (_isBuilt)
            {
                Debug.LogWarning($"{name}: scope was already built.", this);
                return;
            }

            _isBuilt = true;

            var members = new List<MonoBehaviour>();
            Collect(transform, members);
            foreach (var scanRoot in _additionalScanRoots)
            {
                if (scanRoot != null) Collect(scanRoot.transform, members);
            }
            members = members.Distinct().ToList();

            Container = resolver.CreateScope(builder =>
            {
                ConfigureScope(builder);
                foreach (var member in members.OfType<IScopeRegisterable>())
                {
                    SafeInvoke(member, () => member.OnRegister(builder));
                }
            });

            foreach (var member in members)
            {
                SafeInvoke(member, () => Container.Inject(member));
            }

            launchTargets.Add(this);
            foreach (var member in members)
            {
                if (member is AbstractScopeRoot nestedRoot)
                {
                    nestedRoot.ResolveChildren(Container, launchTargets);
                }
                else
                {
                    launchTargets.Add(member);
                }
            }
        }

        /// <summary>最上位のルートとして呼ぶ入口。ツリー全体の注入と登録が済んだ後にOnLaunchを呼ぶ。</summary>
        public void Build(IObjectResolver resolver)
        {
            SafeInvoke(this, () => resolver.Inject(this));

            var launchTargets = new List<MonoBehaviour>();
            ResolveChildren(resolver, launchTargets);

            foreach (var launchable in launchTargets.OfType<IScopeLaunchable>())
            {
                SafeInvoke(launchable, launchable.OnLaunch);
            }
        }
    }
}
