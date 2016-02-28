using System.Web.Configuration;
using justinobney.gymbuddy.api.Interfaces;
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
            });

            For<IRestClient>().Use(context => new RestClient());

            ConfigureLogger(compilationSection.Debug);
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
                    .WriteTo.Exceptionless();
            }

            var log = logConfig.CreateLogger();

            For<ILogger>().Use(log);
        }
    }
}