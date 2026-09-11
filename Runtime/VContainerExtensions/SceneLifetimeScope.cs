using TNRD;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace MyUtils.VContainerExtensions
{
    /// <summary>
    /// シーンのエントリーポイントとなるLifetimeScope。
    /// VContainerのルートコンテナのConfigureを、<see cref="IScopeRoot"/>ツリー全体の
    /// 登録・構築(<see cref="IScopeRoot.OnRegister"/>/<see cref="IScopeRoot.Build"/>)に委譲する。
    /// 具体的なスコープ構成(Game/Playerなど)はSceneScopeRootに差し込むコンポーネント側が持つため、
    /// このクラス自身は型引数を意識しない。
    /// </summary>
    public class SceneLifetimeScope : LifetimeScope
    {
        [Tooltip("シーンのルートとなるIScopeRoot実装(例: GameScopeRoot)")]
        public SerializableInterface<IScopeRoot> SceneScopeRoot;

        protected override void Configure(IContainerBuilder builder)
        {
            SceneScopeRoot.Value.OnRegister(builder);

            // ルートコンテナの構築(ビルド)完了後、ツリー全体の子スコープ構築とOnResolveをまとめて行う。
            builder.RegisterBuildCallback(resolver => SceneScopeRoot.Value.Build(resolver));
        }
    }
}