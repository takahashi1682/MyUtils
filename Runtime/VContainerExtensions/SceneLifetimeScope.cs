using System.Collections.Generic;
using VContainer;
using VContainer.Unity;

namespace MyUtils.VContainerExtensions
{
    /// <summary>
    /// シーン固有のコンテナ初期化プロセスを定義するインターフェース
    /// </summary>
    public interface ISceneInitializable2 : IContainerInitializable2
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
            var targets = new List<ISceneInitializable2>();
            var rootObjects = gameObject.scene.GetRootGameObjects();

            foreach (var obj in rootObjects)
            {
                targets.AddRange(obj.GetComponentsInChildren<ISceneInitializable2>(true));
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