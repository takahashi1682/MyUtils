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
    public class SceneLifetimeScope : LifetimeScope
    {
        [Tooltip("シーンのルートとなるAbstractScopeRoot実装(例: GameScopeRoot)")]
        public AbstractScopeRoot SceneScopeRoot;

        protected override void Configure(IContainerBuilder builder)
        {
            SceneScopeRoot.OnRegister(builder);

            // ルートコンテナの構築(ビルド)完了後、ツリー全体の子スコープ構築とOnLaunchをまとめて行う。
            builder.RegisterBuildCallback(resolver => SceneScopeRoot.Build(resolver));
        }
    }
}