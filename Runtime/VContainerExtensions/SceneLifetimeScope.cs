using TNRD;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace MyUtils.VContainerExtensions
{
    /// <summary>
    /// シーンのエントリーポイントとなるLifetimeScope。
    /// VContainerのルートコンテナのConfigureを、<see cref="AbstractScopeRoot"/>ツリー全体の
    /// 登録・構築(<see cref="AbstractScopeRoot.OnRegister"/>/<see cref="AbstractScopeRoot.Build"/>)に委譲する。
    /// 具体的なスコープ構成(Game/Playerなど)はSceneScopeRootに差し込むコンポーネント側が持つ。
    /// </summary>
    // LifetimeScopeの実行順(-5000)が派生クラスに引き継がれることに頼らず、明示してどのAwakeよりも先に構築する
    [DefaultExecutionOrder(-5000)]
    public class SceneLifetimeScope : LifetimeScope
    {
        [Tooltip("シーンのルートとなるAbstractScopeRoot実装(例: GameScopeRoot)")]
        public AbstractScopeRoot SceneScopeRoot;

        protected override void Awake()
        {
            // 親にRootLifetimeScopeを指定していても、ルートが未生成だとVContainerはこのスコープの構築を後回しにする
            // (ルートはシーン読み込み完了後に作られる)。その場合、他のコンポーネントのAwakeには注入が間に合わない。
            // そのため、先にルートを作っておき、このAwakeの中で構築を終わらせる。
            VContainerSettings.Instance?.GetOrCreateRootLifetimeScopeInstance();

            base.Awake();
        }

        protected override void Configure(IContainerBuilder builder)
        {
            // 登録するものがないシーンはSceneScopeRootを省略でき、autoInjectGameObjectsだけで動く。
            if (SceneScopeRoot == null)
            {
                Debug.LogWarning($"{name}: SceneScopeRoot が未設定です。autoInjectGameObjects のみで動作します。", this);
                return;
            }

            SceneScopeRoot.OnRegister(builder);

            // ルートコンテナの構築(ビルド)完了後、ツリー全体の子スコープ構築と注入をまとめて行う。
            builder.RegisterBuildCallback(resolver => SceneScopeRoot.Build(resolver));
        }
    }
}