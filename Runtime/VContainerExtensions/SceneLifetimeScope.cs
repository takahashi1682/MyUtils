using System.Collections.Generic;
using VContainer;
using VContainer.Unity;

namespace MyUtils.VContainerExtensions
{
    public interface IContainerInitializable
    {
        /// <summary>
        /// 依存性注入の登録時に呼び出される初期化処理
        /// </summary>
        /// <param name="builder"></param>
        void OnRegister(IContainerBuilder builder);

        /// <summary>
        /// 依存性注入の解決時に呼び出される初期化処理
        /// </summary>
        /// <param name="resolver"></param>
        void OnResolve(IObjectResolver resolver);
    }

    /// <summary>
    /// シーン固有のコンテナ初期化プロセスを定義するインターフェース
    /// </summary>
    public interface ISceneInitializable : IContainerInitializable
    {
    }

    /// <summary>
    /// 全てのシーンに配置するLifetimeScope。
    /// シーン内のISceneContainerInitializableを自動検出し、独立したコンテナ構築フローを提供します。
    /// </summary>
    public class SceneLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            var targets = new List<ISceneInitializable>();
            var rootObjects = gameObject.scene.GetRootGameObjects();

            foreach (var obj in rootObjects)
            {
                targets.AddRange(obj.GetComponentsInChildren<ISceneInitializable>(true));
            }

            builder.RegisterBuildCallback(resolver =>
            {
                var scopedResolver = resolver.CreateScope(newBuilder =>
                {
                    foreach (var target in targets)
                    {
                        target.OnRegister(newBuilder);
                    }
                });

                foreach (var target in targets)
                {
                    target.OnResolve(scopedResolver);
                }
            });
        }
    }
}