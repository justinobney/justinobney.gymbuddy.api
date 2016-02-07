using System.Web.Http;
using System.Web.Http.Cors;
using justinobney.gymbuddy.api.Filters;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace justinobney.gymbuddy.api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // web API configuration and services
            var cors = new EnableCorsAttribute("*", "*", "*");
            config.EnableCors(cors);
            
            // camel case formatters
            var settings = config.Formatters.JsonFormatter.SerializerSettings;
            settings.Formatting = Formatting.Indented;
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            // web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // add exception filters
            config.Filters.Add(new ValidationExceptionFilter());
            config.Filters.Add(new AuthorizationExceptionFilter());

            config.SuppressHostPrincipal();
        }
    }
}
