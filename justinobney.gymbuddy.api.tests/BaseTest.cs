using justinobney.gymbuddy.api.DependencyResolution;
using justinobney.gymbuddy.api.tests.DependencyResolution;
using justinobney.gymbuddy.api.tests.Helpers;
using MediatR;
using NUnit.Framework;
using StructureMap;

namespace justinobney.gymbuddy.api.tests
{
    [TestFixture]
    public class BaseTest
    {
        protected IMediator Mediator;
        protected CoreTestContext Context;
        private Container _container;

        [TestFixtureSetUp]
        public void ConfigIoC()
        {
            var registry = new Registry();
            registry.IncludeRegistry<DefaultRegistry>();
            registry.IncludeRegistry<GenericCrudRegistry>();
            registry.IncludeRegistry<FakeNotificationRegistry>();
            registry.IncludeRegistry<FakeEntityFrameworkRegistry>();

            _container = new Container(registry);
            _container.AssertConfigurationIsValid();
            Context = new CoreTestContext(_container);

            Mediator = _container.GetInstance<IMediator>();
        }

        [SetUp]
        public void TestSetup()
        {
            Context.ClearAll();
            Context.ResetIoC();
        }

    }
}
