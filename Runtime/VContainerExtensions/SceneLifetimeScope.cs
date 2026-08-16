using TNRD;
using VContainer;
using VContainer.Unity;

namespace MyUtils.VContainerExtensions
{
    public class SceneLifetimeScope : LifetimeScope
    {
        public SerializableInterface<IContainerInitializable> ScopeRoot;

        protected override void Configure(IContainerBuilder builder)
        {
            ScopeRoot.Value.OnRegister(builder);

            builder.RegisterBuildCallback(resolver =>
            {
                ScopeRoot.Value.OnResolve(resolver);
            });
        }
    }
}