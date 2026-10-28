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

    /// <summary>
    /// 自身と子孫のMonoBehaviourを集めてスコープを作り、OnRegisterと[Inject]を行う。
    /// 構築はどのAwakeよりも先に終わるので、注入されたメンバーはAwake以降で使える。
    /// </summary>
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

        // Instantiateで生成した直後のオブジェクトを一時的に置く、非アクティブな親
        private Transform _spawnBuffer;

        /// <summary>親のコンテナへの登録処理。</summary>
        public virtual void OnRegister(IContainerBuilder builder) { }

        /// <summary>このスコープ内で共有する依存の登録処理。</summary>
        protected virtual void ConfigureScope(IContainerBuilder builder) { }

        /// <summary>子孫以外に対象に加えるGameObject(既定はInspectorで設定したもの)。</summary>
        protected virtual IEnumerable<GameObject> AdditionalScanRoots => _additionalScanRoots;

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

        // 子スコープを作って全員に注入する。入れ子のルートは、そのルートが続きを担当する。
        private void ResolveChildren(IObjectResolver resolver)
        {
            if (_isBuilt)
            {
                Debug.LogWarning($"{name}: scope was already built.", this);
                return;
            }

            _isBuilt = true;

            var members = new List<MonoBehaviour>();
            Collect(transform, members);
            foreach (var scanRoot in AdditionalScanRoots)
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

            foreach (var nestedRoot in members.OfType<AbstractScopeRoot>())
            {
                nestedRoot.ResolveChildren(Container);
            }
        }

        /// <summary>最上位のルートとして呼ぶ入口。ツリー全体の登録と注入を行う。</summary>
        public void Build(IObjectResolver resolver)
        {
            SafeInvoke(this, () => resolver.Inject(this));
            ResolveChildren(resolver);
        }

        /// <summary>
        /// prefabを生成し、このスコープの子スコープとして登録と[Inject]を済ませてから有効化する。
        /// 生成したオブジェクトのAwake/OnEnable/Startは、注入が終わった後に呼ばれる。
        /// prefabがプレハブの子についている場合は、プレハブ全体を生成して対応するコンポーネントを返す。
        /// position/rotationは生成したルートに適用し、省略した場合はプレハブのルート自身の位置・回転を使う。
        /// </summary>
        public T Instantiate<T>(T prefab, Vector3? position = null, Quaternion? rotation = null,
            Transform parent = null)
            where T : AbstractScopeRoot
        {
            // プレハブの子についている場合もプレハブ全体を生成する(シーン上のオブジェクトはそれ自体だけ)
            var source = prefab.gameObject.scene.IsValid() ? prefab.transform : prefab.transform.root;
            var instance = Instantiate(source.gameObject, position, rotation, parent);
            return FindCounterpart(source, prefab, instance.transform);
        }

        // sourceRoot以下のtargetと同じ階層位置にある、instanceRoot以下のコンポーネントを返す
        private static T FindCounterpart<T>(Transform sourceRoot, T target, Transform instanceRoot)
            where T : Component
        {
            var path = new Stack<int>();
            for (var t = target.transform; t != sourceRoot; t = t.parent)
            {
                path.Push(t.GetSiblingIndex());
            }

            var current = instanceRoot;
            while (path.Count > 0)
            {
                current = current.GetChild(path.Pop());
            }

            var index = Array.IndexOf(target.GetComponents<T>(), target);
            return current.GetComponents<T>()[index];
        }

        /// <summary>
        /// prefabを生成し、[Inject]を済ませてから有効化する。
        /// 中にAbstractScopeRootがあればそのスコープを作り、無ければこのスコープのコンテナで注入する
        /// (後者の場合、コンテナは構築済みなのでOnRegisterは呼ばれない)。
        /// position/rotationを省略した場合は、prefab自身の位置・回転を使う。
        /// </summary>
        public GameObject Instantiate(GameObject prefab, Vector3? position = null, Quaternion? rotation = null,
            Transform parent = null)
        {
            if (Container == null)
            {
                throw new InvalidOperationException($"{name}: scope is not built yet.");
            }

            // 非アクティブな親の下に生成し、注入が終わるまでAwakeを止める
            var instance = UnityEngine.Object.Instantiate(prefab,
                position ?? prefab.transform.position,
                rotation ?? prefab.transform.rotation,
                GetSpawnBuffer());

            var members = new List<MonoBehaviour>();
            Collect(instance.transform, members);
            foreach (var member in members)
            {
                if (member is AbstractScopeRoot nestedRoot)
                {
                    nestedRoot.Build(Container);
                }
                else
                {
                    SafeInvoke(member, () => Container.Inject(member));
                }
            }

            // 親を付け替えた時点で有効になり、Awake/OnEnable/Startが呼ばれる
            instance.transform.SetParent(parent, true);
            return instance;
        }

        private Transform GetSpawnBuffer()
        {
            if (_spawnBuffer != null) return _spawnBuffer;

            var buffer = new GameObject("SpawnBuffer");
            buffer.SetActive(false);
            buffer.transform.SetParent(transform, false);
            _spawnBuffer = buffer.transform;
            return _spawnBuffer;
        }
    }
}
