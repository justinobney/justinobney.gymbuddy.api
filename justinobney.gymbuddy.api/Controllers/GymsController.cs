using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using AutoMapper.QueryableExtensions;
using justinobney.gymbuddy.api.Data.Gyms;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Requests.Generic;
using justinobney.gymbuddy.api.Responses;
using MediatR;

namespace justinobney.gymbuddy.api.Controllers
{
    public class GymsController : AuthenticatedController
    {
        public GymsController(IMediator mediator):base(mediator)
        {
        }

        // GET: api/Gyms
        [ResponseType(typeof(IEnumerable<GymListing>))]
        public IHttpActionResult GetGyms()
        {
            var gyms = _mediator.Send(new GetAllByPredicateQuery<Gym>())
                .ProjectTo<GymListing>(MappingConfig.Config)
                .ToList();

            return Ok(gyms);
        }

        // GET: api/Gyms/5
        [ResponseType(typeof(GymListing))]
        public IHttpActionResult GetGym(long id)
        {
            var request = new GetAllByPredicateQuery<Gym>
            {
                Predicate = u => u.Id == id
            };

            var gym = _mediator.Send(request)
                .ProjectTo<GymListing>(MappingConfig.Config)
                .FirstOrDefault();

            if (gym == null)
            {
                return NotFound();
            }

            return Ok(gym);
        }

        // GET: api/Gyms/5/Peek-Users/12345
        [ResponseType(typeof(IEnumerable<ProfileListing>))]
        [Route("api/Gyms/{id}/Peek-Users")]
        public IHttpActionResult GetGymUsersPeek(long id)
        {
            if (CurrentUser == null)
            {
                return Unauthorized();
            }

            var request = new GetAllByPredicateQuery<User>(UserPredicates.RestrictMember(CurrentUser, id));
            var users = _mediator.Send(request)
                    .ProjectTo<ProfileListing>(MappingConfig.Config);

            if (users == null)
            {
                return NotFound();
            }

            return Ok(users);
        }
        
    }
}