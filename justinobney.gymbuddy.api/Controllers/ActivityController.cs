using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using justinobney.gymbuddy.api.Data.Notifications;
using justinobney.gymbuddy.api.Requests.Generic;
using justinobney.gymbuddy.api.Requests.Users;
using MediatR;

namespace justinobney.gymbuddy.api.Controllers
{
    public class ActivityController : AuthenticatedController
    {
        private readonly Mediator _mediator;

        public ActivityController(Mediator mediator) : base(mediator)
        {
            _mediator = mediator;
        }

        // GET: api/Users
        [ResponseType(typeof(IEnumerable<Notification>))]
        public IHttpActionResult GetActivity()
        {
            var reuest = new GetAllByPredicateQuery<Notification>
            {
                Predicate = x => x.UserId == CurrentUser.Id
            };

            var notifications = _mediator.Send(reuest)
                .OrderByDescending(x=>x.CreatedAt)
                .ToList();

            return Ok(notifications);
        }
        
        [HttpPost]
        [Route("api/Mark-Seen")]
        public IHttpActionResult MarkSeen(UpdateDeviceCommand command)
        {
//            if (CurrentUser == null || CurrentUser.Devices.All(d => d.DeviceId != command.DeviceId))
//            {
//                return Unauthorized();
//            }
//
//            _mediator.Send(command);
            return Ok();
        }

    }
}