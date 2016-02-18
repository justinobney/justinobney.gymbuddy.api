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

        [OneTimeSetUp]
        public void SetUp()
        {
            var registry = new Registry();
            registry.IncludeRegistry<DefaultRegistry>();
            registry.IncludeRegistry<GenericCrudRegistry>();
            registry.IncludeRegistry<FakeEntityFrameworkRegistry>();

            _container = new Container(registry);
            Context = new CoreTestContext(_container);
            Mediator = _container.GetInstance<IMediator>();
        }

        [TearDown]
        public void TearDown()
        {
            Context.ClearAll();
        }

    }
}
