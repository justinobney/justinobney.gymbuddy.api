using System.Configuration;
using System.Web.Configuration;
using CloudinaryDotNet;
using justinobney.gymbuddy.api.Interfaces;
using justinobney.gymbuddy.api.Requests.Decorators;
using MediatR;
using NSubstitute;
using RestSharp;
using Serilog;
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
                }
                else
                {
                    For<IRestClient>().Use(context => new RestClient());

                    var cloud = ConfigurationManager.AppSettings.Get("Cloudinary-Cloud");
                    var key = ConfigurationManager.AppSettings.Get("Cloudinary-ApiKey");
                    var secret = ConfigurationManager.AppSettings.Get("Cloudinary-ApiSecret");
                    var account = new Account(cloud,key,secret);
                    For<Cloudinary>().Use(new Cloudinary(account));
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