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
        [Route("api/Gyms/{id}/Peek-Users/{deviceId}")]
        public IHttpActionResult GetGymUsersPeek(long id, string deviceId)
        {
            var requestingUser = _mediator.Send(new GetAllByPredicateQuery<User>
            {
                Predicate = u => u.Devices.Any(d => d.DeviceId == deviceId)
            })
            .FirstOrDefault();

            var users = _mediator.Send(new GetAllByPredicateQuery<User>
            {
                //FilterByGender(requestingUser, u)

                Predicate = u => u.Gyms.Any(g => g.Id == id)
                                 && u.FilterFitnessLevel <= requestingUser.FitnessLevel
                                 && u.FitnessLevel >= requestingUser.FilterFitnessLevel
                                 && u.Id != requestingUser.Id
            })
                .ProjectTo<ProfileListing>(MappingConfig.Config);

            if (users == null)
            {
                return NotFound();
            }

            return Ok(users);
        }

        //private bool FilterByGender(User requestingUser, User user)
        //{
        //    var LookingForOpposites = (int) requestingUser.FilterGender == (int) user.Gender &&
        //                              (int) requestingUser.Gender == (int) user.FilterGender;

        //    var MatchWhenRequestingBoth = requestingUser.FilterGender == GenderFilter.Both &&
        //                                  (int)user.FilterGender == (int)requestingUser.Gender;

        //    var MatchWhenUserBoth = user.FilterGender == GenderFilter.Both &&
        //                                  (int)requestingUser.FilterGender == (int)user.Gender;

        //    var BothAcceptBoth = user.FilterGender == GenderFilter.Both &&
        //                         requestingUser.FilterGender == GenderFilter.Both;

        //    return LookingForOpposites || MatchWhenRequestingBoth || MatchWhenUserBoth || BothAcceptBoth;
        //}
        

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