using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using justinobney.gymbuddy.api.Data.Appointments;
using justinobney.gymbuddy.api.Requests.Guests;
using MediatR;

namespace justinobney.gymbuddy.api.Controllers
{
    public class RequestsController : AuthenticatedController
    {
        private readonly Mediator _mediator;

        public RequestsController(Mediator mediator) : base(mediator)
        {
            _mediator = mediator;
        }

        // GET: api/Users
        [ResponseType(typeof(IEnumerable<AppointmentGuest>))]
        public IHttpActionResult GetActivity()
        {
            var notifications = _mediator.Send(new GetOpenRequestsForUserQuery {UserId = CurrentUser.Id})
                .Include(x=>x.TimeSlot.Time)
                .OrderByDescending(x => x.TimeSlot.Time)
                .ToList();

            return Ok(notifications);
        }
    }
}