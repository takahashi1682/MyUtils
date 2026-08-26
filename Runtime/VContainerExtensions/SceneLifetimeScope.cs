using VContainer;
using VContainer.Unity;

namespace MyUtils.VContainerExtensions
{
    public class SceneLifetimeScope : LifetimeScope
    {
        public AbstractScopeRoot ScopeRoot;

        protected override void Configure(IContainerBuilder builder)
        {
            ScopeRoot.OnRegister(builder);
            builder.RegisterBuildCallback(resolver => ScopeRoot.Run(resolver));
        }
    }
}