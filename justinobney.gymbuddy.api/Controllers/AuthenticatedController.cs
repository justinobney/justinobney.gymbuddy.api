using System.Data.Entity;
using System.Linq;
using System.Web.Http;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Requests.Generic;
using MediatR;

namespace justinobney.gymbuddy.api.Controllers
{
    public class AuthenticatedController : ApiController
    {
        protected readonly IMediator _mediator;

        public User CurrentUser
        {
            get
            {
                if (!Request.Headers.Contains("device-id"))
                    return null;

                var deviceId = Request.Headers.GetValues("device-id").FirstOrDefault();
                var request = new GetAllByPredicateQuery<User>(UserPredicates.RestrictByDeviceId(deviceId));
                return _mediator.Send(request)
                    .Include(x=>x.Devices)
                    .Include(x => x.Gyms)
                    .FirstOrDefault();
            }
        }

        public string FacebookUserId => Request.Headers.FirstOrDefault(x => x.Key == "fb-user-id").Value?.FirstOrDefault();

        public string ClientVersion => Request.Headers.FirstOrDefault(x => x.Key == "client-version").Value?.FirstOrDefault();

        public AuthenticatedController(IMediator mediator)
        {
            _mediator = mediator;
        }
    }
}