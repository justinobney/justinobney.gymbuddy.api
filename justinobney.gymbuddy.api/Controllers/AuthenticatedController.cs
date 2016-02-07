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
                return _mediator.Send(new GetAllByPredicateQuery<User>
                {
                    Predicate = u => u.Devices.Any(d => d.DeviceId == deviceId)
                })
                    .FirstOrDefault();
            }
        }

        public AuthenticatedController(IMediator mediator)
        {
            _mediator = mediator;
        }
    }
}