using System.Configuration;
using System.Web.Configuration;
using CloudinaryDotNet;
using Hangfire;
using Hangfire.SqlServer;
using justinobney.gymbuddy.api.Helpers;
using justinobney.gymbuddy.api.Interfaces;
using justinobney.gymbuddy.api.Requests.Decorators;
using MediatR;
using NSubstitute;
using RestSharp;
using Serilog;
using Stream;
using StructureMap;
using StructureMap.Graph;

namespace justinobney.gymbuddy.api.DependencyResolution.Registries
{
    public class ProductionInfrastructureRegistry : Registry
    {
        public ProductionInfrastructureRegistry()
        {
            var compilationSection = (CompilationSection)ConfigurationManager.GetSection(@"system.web/compilation");

            Scan(scan =>
            {
                scan.TheCallingAssembly();
                scan.AssemblyContainingType(typeof(IEntity));
                scan.AssemblyContainingType(typeof(IPostRequestHandler<,>));

                if (compilationSection.Debug)
                {
                    var restClient = Substitute.For<RestClient>();
                    For<IRestClient>().Use(context => restClient);

                    var cloudinary = Substitute.For<Cloudinary>(new Account("fake", "fake", "fake"));
                    For<Cloudinary>().Use(context => cloudinary);

                    For<IImageUploader>().Use(Substitute.For<IImageUploader>());

                    For<IBackgroundJobClient>().Use(Substitute.For<IBackgroundJobClient>());

                    var streamClient = Substitute.For<StreamClient>("YOUR_API_KEY", "API_KEY_SECRET", null);
                    For<StreamClient>().Use(context => streamClient);
                }
                else
                {
                    For<IBackgroundJobClient>()
                        .Use(new BackgroundJobClient(new SqlServerStorage("AppContext")));

                    For<IRestClient>().Use(context => new RestClient());

                    var cloud = ConfigurationManager.AppSettings.Get("Cloudinary-Cloud");
                    var cloudinaryKey = ConfigurationManager.AppSettings.Get("Cloudinary-ApiKey");
                    var cloudinarySecret = ConfigurationManager.AppSettings.Get("Cloudinary-ApiSecret");
                    var account = new Account(cloud, cloudinaryKey, cloudinarySecret);
                    For<Cloudinary>().Use(context => new Cloudinary(account));

                    var streamKey = ConfigurationManager.AppSettings.Get("Stream-ApiKey");
                    var streamSecret = ConfigurationManager.AppSettings.Get("Stream-ApiSecret");

                    For<StreamClient>().Use(context => new StreamClient(streamKey, streamSecret, null));

                    For<IImageUploader>().Use<ImageUploader>();
                }


                ConfigureNotifications(scan);
            });
            
            ConfigureLogger(compilationSection.Debug);

        }

        private void ConfigureNotifications(IAssemblyScanner scan)
        {
            scan.AddAllTypesOf(typeof(IPostRequestHandler<,>));
            For(typeof(IRequestHandler<,>)).DecorateAllWith(typeof(PostRequestHandler<,>));
        }

        private void ConfigureLogger(bool isDebugEnabled)
        {
            var logConfig = new LoggerConfiguration();

            if (isDebugEnabled)
            {
                logConfig
                    .WriteTo.ColoredConsole()
                    .WriteTo.Trace();
            }
            else
            {
                logConfig
                    .WriteTo.Loggly();
            }

            var log = logConfig.CreateLogger();

            For<ILogger>().Use(log);
        }
    }
}