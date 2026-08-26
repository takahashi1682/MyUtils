using VContainer;
using VContainer.Unity;

namespace MyUtils.VContainerExtensions
{
    public class SceneLifetimeScope : LifetimeScope
    {
        public AbstractScopeRoot SceneScopeRoot;

        protected override void Configure(IContainerBuilder builder)
        {
            SceneScopeRoot.OnRegister(builder);
            builder.RegisterBuildCallback(resolver => SceneScopeRoot.Build(resolver));
        }
    }
}