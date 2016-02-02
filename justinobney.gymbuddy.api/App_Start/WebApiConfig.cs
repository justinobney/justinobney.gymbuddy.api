using System.Web.Http;
using justinobney.gymbuddy.api.Filters;

namespace justinobney.gymbuddy.api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

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
