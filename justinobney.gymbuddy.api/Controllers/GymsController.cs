using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using AutoMapper.QueryableExtensions;
using justinobney.gymbuddy.api.Data.Gyms;
using justinobney.gymbuddy.api.Requests.Generic;
using justinobney.gymbuddy.api.Responses;
using MediatR;

namespace justinobney.gymbuddy.api.Controllers
{
    public class GymsController : ApiController
    {
        private readonly Mediator _mediator;

        public GymsController(Mediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/Gyms
        [ResponseType(typeof(IEnumerable<GymListing>))]
        public IHttpActionResult GetGyms()
        {
            var users = _mediator.Send(new GetByPredicateQuery<Gym>())
                .ProjectTo<GymListing>(MappingConfig.Config)
                .ToList();

            return Ok(users);
        }

        // GET: api/Gyms/5
        [ResponseType(typeof(GymListing))]
        public IHttpActionResult GetGym(long id)
        {
            var request = new GetByPredicateQuery<Gym>
            {
                Predicate = u => u.Id == id
            };

            var user = _mediator.Send(request)
                .ProjectTo<GymListing>(MappingConfig.Config)
                .FirstOrDefault();

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        // POST: api/Gyms
        //[ResponseType(typeof(Gym))]
        //public async Task<IHttpActionResult> PostGym(CreateGymCommand command)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    var user = await _mediator.SendAsync(command);
        //    return CreatedAtRoute("DefaultApi", new { id = user.Id }, user);
        //}


        // PUT: api/Gyms/5
        //[ResponseType(typeof(void))]
        //public async Task<IHttpActionResult> PutGym(long id, UpdateGymCommand command)
        //{
        //    if (id != command.Id)
        //    {
        //        return BadRequest();
        //    }

        //    await _mediator.SendAsync(command);
        //    return StatusCode(HttpStatusCode.NoContent);
        //}

        //TODO: Create generic delete command
        // DELETE: api/Gyms/5
        //[ResponseType(typeof(Gym))]
        //public async Task<IHttpActionResult> DeleteGym(long id)
        //{
        //    var user = await _mediator.SendAsync(new DeleteGymCommand { Id = id });
        //    return Ok(user);
        //}

    }
}