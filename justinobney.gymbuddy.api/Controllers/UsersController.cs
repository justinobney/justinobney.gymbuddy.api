using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Requests.Users;
using justinobney.gymbuddy.api.Responses;
using MediatR;

namespace justinobney.gymbuddy.api.Controllers
{
    public class UsersController : ApiController
    {
        private readonly Mediator _mediator;

        public UsersController(Mediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/Users
        [ResponseType(typeof(IEnumerable<ProfileListing>))]
        public IHttpActionResult GetUsers()
        {
            var users = _mediator.Send(new GetUsersQuery());
            return Ok(users);
        }

        // GET: api/Users/5
        [ResponseType(typeof(ProfileListing))]
        public async Task<IHttpActionResult> GetUser(string id)
        {
            //var user = await _mediator.SendAsync(new GetByIdQuery<User> {Id = id});
            var user = await _mediator.SendAsync(new GetUserQuery {DeviceId = id});
            if (user == null)
            {
                return NotFound();
            }
            
            return Ok(user);
        }

        // POST: api/Users
        [ResponseType(typeof(User))]
        public async Task<IHttpActionResult> PostUser(CreateUserCommand command)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _mediator.SendAsync(command);
            return CreatedAtRoute("DefaultApi", new { id = user.Id }, user);
        }


        // PUT: api/Users/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutUser(long id, UpdateUserCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest();
            }

            await _mediator.SendAsync(command);
            return StatusCode(HttpStatusCode.NoContent);
        }

        // DELETE: api/Users/5
        [ResponseType(typeof(User))]
        public async Task<IHttpActionResult> DeleteUser(long id)
        {
            var user = await _mediator.SendAsync(new DeleteUserCommand {Id = id});
            return Ok(user);
        }
        
    }
}