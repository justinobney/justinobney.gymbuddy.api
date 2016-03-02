using System.Web.Configuration;
using justinobney.gymbuddy.api.Interfaces;
using justinobney.gymbuddy.api.Requests.Decorators;
using MediatR;
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
            var compilationSection = (CompilationSection)System.Configuration.ConfigurationManager.GetSection(@"system.web/compilation");

            Scan(scan =>
            {
                scan.TheCallingAssembly();
                scan.AssemblyContainingType(typeof(IEntity));
                scan.AssemblyContainingType(typeof(IPostRequestHandler<,>));
                if (!compilationSection.Debug)
                {
                    ConfigureNotifications(scan);
                }
            });

            For<IRestClient>().Use(context => new RestClient());

            ConfigureLogger(compilationSection.Debug);
        }

        private void ConfigureNotifications(IAssemblyScanner scan)
        {
            scan.AddAllTypesOf(typeof(IPostRequestHandler<,>));
            var handlerType = For(typeof(IRequestHandler<,>));
            handlerType.DecorateAllWith(typeof(PostRequestHandler<,>));
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