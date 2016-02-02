using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http.Filters;
using FluentValidation;
using justinobney.gymbuddy.api.Responses;
using Newtonsoft.Json;

namespace justinobney.gymbuddy.api.Filters
{
    public class ValidationExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.Exception is ValidationException)
            {
                actionExecutedContext.Response = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent(
                        JsonConvert.SerializeObject(
                            ((ValidationException) actionExecutedContext.Exception).Errors.Select(e => new ApiError(e))),
                        Encoding.UTF8,
                        "application/json"
                        )
                };
            }
        }
    }
}