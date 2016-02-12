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
        private readonly IMediator _mediator;

        public User CurrentUser
        {
            get
            {
                if (!Request.Headers.Contains("device-id"))
                    return null;

                var deviceId = Request.Headers.GetValues("device-id").FirstOrDefault();
                var request = new GetAllByPredicateQuery<User>(UserPredicates.RestrictByDeviceId(deviceId));
                return _mediator.Send(request)
                    .Include(u => u.Gyms)
                    .FirstOrDefault();
            }
        }

        public AuthenticatedController(IMediator mediator)
        {
            _mediator = mediator;
        }
    }
}