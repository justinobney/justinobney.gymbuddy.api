using System.Web.Http;
using System.Web.Http.Cors;
using justinobney.gymbuddy.api.Filters;

namespace justinobney.gymbuddy.api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            var cors = new EnableCorsAttribute("*", "*", "*");
            config.EnableCors(cors);

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Filters.Add(new ValidationExceptionFilter());
            config.Filters.Add(new AuthorizationExceptionFilter());
        }
    }
}
