using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using AutoMapper.QueryableExtensions;
using justinobney.gymbuddy.api.Requests.Guests;
using justinobney.gymbuddy.api.Responses;
using MediatR;

namespace justinobney.gymbuddy.api.Controllers
{
    public class RequestsController : AuthenticatedController
    {

        public RequestsController(IMediator mediator) : base(mediator)
        {
            
        }

        // GET: api/Users
        [ResponseType(typeof(IEnumerable<AppointmentGuestListing>))]
        public IHttpActionResult GetRequests()
        {
            var notifications = _mediator.Send(new GetOpenRequestsForUserQuery {UserId = CurrentUser.Id})
                .Include(x=>x.TimeSlot)
                .Include(x=>x.User)
                .OrderByDescending(x => x.TimeSlot.Time)
                .ProjectTo<AppointmentGuestListing>(MappingConfig.Config);

            return Ok(notifications);
        }

        [Route("api/Activity")]
        [ResponseType(typeof(IEnumerable<AppointmentGuestListing>))]
        public IHttpActionResult GetActivity()
        {
            return GetRequests();
        }
    }
}