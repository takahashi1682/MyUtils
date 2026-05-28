using VContainer;

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
}