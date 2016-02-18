using justinobney.gymbuddy.api.DependencyResolution;
using justinobney.gymbuddy.api.tests.DependencyResolution;
using MediatR;
using NUnit.Framework;
using StructureMap;

namespace justinobney.gymbuddy.api.tests
{
    [TestFixture]
    public class BaseTest
    {
        protected IMediator Mediator;

        [OneTimeSetUp]
        public void SetUp()
        {
            var registry = new Registry();
            registry.IncludeRegistry<DefaultRegistry>();
            registry.IncludeRegistry<GenericCrudRegistry>();
            registry.IncludeRegistry<FakeEntityFrameworkRegistry>();

            var container = new Container(registry);
            Mediator = container.GetInstance<IMediator>();
        }
    }
}
