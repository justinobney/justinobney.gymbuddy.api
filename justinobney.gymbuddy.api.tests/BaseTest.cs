using CloudinaryDotNet;
using Hangfire;
using justinobney.gymbuddy.api.Data;
using justinobney.gymbuddy.api.DependencyResolution.Registries;
using justinobney.gymbuddy.api.tests.DependencyResolution;
using justinobney.gymbuddy.api.tests.Helpers;
using MediatR;
using NSubstitute;
using NUnit.Framework;
using RestSharp;
using Serilog;
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

            // ProductionInfrastructureRegistry
            var restClient = Substitute.For<RestClient>();
            registry.For<IRestClient>().Use(restClient);

            var account = new Account("fake", "fake", "fake");
            var cloudinary = Substitute.For<Cloudinary>(account);
            registry.For<Cloudinary>().Use(context => cloudinary);

            var log = new LoggerConfiguration()
                .WriteTo.ColoredConsole()
                .CreateLogger();


            registry.For<ILogger>().Use(log);

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
            Context.GetInstance<AppContext>().ClearReceivedCalls();
        }
    }
}
