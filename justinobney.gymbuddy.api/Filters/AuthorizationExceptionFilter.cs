using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using justinobney.gymbuddy.api.Exceptions;

namespace justinobney.gymbuddy.api.Filters
{
    public class AuthorizationExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.Exception is AuthorizationException)
            {
                actionExecutedContext.Response = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.Unauthorized,
                    Content = new StringContent("You do not have permission...")
                };
            }
        }
    }
}