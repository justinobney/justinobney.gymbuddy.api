﻿using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using AutoMapper.QueryableExtensions;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Requests.Generic;
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
            var request = new GetByPredicateQuery<User>
            {
                Predicate = u => true
            };
            var users = _mediator.Send(request)
                .ProjectTo<ProfileListing>(MappingConfig.Config)
                .ToList();
            return Ok(users);
        }

        // GET: api/Users/5
        [ResponseType(typeof(ProfileListing))]
        public IHttpActionResult GetUser(string id)
        {
            var request = new GetByPredicateQuery<User>
            {
                Predicate = u => u.Devices.Any(device => device.DeviceId == id)
            };

            var user = _mediator.Send(request)
                .ProjectTo<ProfileListing>(MappingConfig.Config)
                .FirstOrDefault();

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