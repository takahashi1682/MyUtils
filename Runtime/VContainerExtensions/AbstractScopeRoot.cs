using UnityEngine;
using VContainer;

namespace MyUtils.VContainerExtensions
{
    public interface IScopeInitializable : IContainerInitializable
    {
    }

    public abstract class AbstractScopeRoot<T> : MonoBehaviour, ISceneInitializable
        where T : IContainerInitializable
    {
        public IObjectResolver Container { get; protected set; }

        public virtual void OnRegister(IContainerBuilder builder) { }

        public virtual void OnResolve(IObjectResolver resolver)
        {
            // プレイヤー配下の IPlayerContainerInitializable を取得
            var targets = gameObject.GetComponentsInChildren<T>(true);

            // プレイヤー専用のコンテナを生成
            Container = resolver.CreateScope(newBuilder =>
            {
                ConfigureScope(newBuilder);

                // その他、IPlayerContainerInitializable を実装するコンポーネントの登録
                foreach (var target in targets)
                {
                    target.OnRegister(newBuilder);
                }
            });

            // 構築されたプレイヤーコンテナを用いて、各コンポーネントの解決を行う
            foreach (var target in targets)
            {
                target.OnResolve(Container);
            }
        }

        protected abstract void ConfigureScope(IContainerBuilder builder);
    }
}