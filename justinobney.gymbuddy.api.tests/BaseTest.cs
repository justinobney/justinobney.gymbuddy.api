using CloudinaryDotNet;
using Hangfire;
using justinobney.gymbuddy.api.Data;
using justinobney.gymbuddy.api.DependencyResolution.Registries;
using justinobney.gymbuddy.api.Helpers;
using justinobney.gymbuddy.api.Interfaces;
using justinobney.gymbuddy.api.tests.DependencyResolution;
using justinobney.gymbuddy.api.tests.Helpers;
using MediatR;
using NSubstitute;
using NUnit.Framework;
using RestSharp;
using Serilog;
using Stream;
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

            var fakeImageUploader = Substitute.For<IImageUploader>();
            fakeImageUploader.UploadFromDataUri(Arg.Any<string>())
                .Returns(info => $"url://{info[0]}");

            registry.For<IImageUploader>().Use(fakeImageUploader);

            var account = new Account("fake", "fake", "fake");
            var cloudinary = Substitute.For<Cloudinary>(account);
            registry.For<Cloudinary>().Use(context => cloudinary);

            var streamClient = Substitute.For<StreamClient>("YOUR_API_KEY", "API_KEY_SECRET", null);
            registry.For<StreamClient>().Use(streamClient);

            registry.For<IStreamClientProxy>().Use<StreamClientProxy>();

            var notifier = Substitute.For<IPushNotifier>();
            registry.For<IPushNotifier>().Use(notifier);

            registry.For<IBackgroundJobClient>().Use(Substitute.For<IBackgroundJobClient>());

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
            Context.GetInstance<IBackgroundJobClient>().ClearReceivedCalls();
            Context.GetInstance<IImageUploader>().ClearReceivedCalls();
            Context.GetInstance<IPushNotifier>().ClearReceivedCalls();
        }

        protected void AssertSaveChanges(int count)
        {
            Context.GetInstance<AppContext>().Received(count).SaveChanges();
        }
    }
}
