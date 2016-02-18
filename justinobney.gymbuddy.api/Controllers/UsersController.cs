﻿using System.Collections.Generic;
using System.Linq;
using System.Net;
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
    public class UsersController : AuthenticatedController
    {
        private readonly Mediator _mediator;

        public UsersController(Mediator mediator):base(mediator)
        {
            _mediator = mediator;
        }

        // GET: api/Users
        [ResponseType(typeof(IEnumerable<ProfileListing>))]
        public IHttpActionResult GetUsers()
        {
            var users = _mediator.Send(new GetAllByPredicateQuery<User>())
                .ProjectTo<ProfileListing>(MappingConfig.Config)
                .ToList();

            return Ok(users);
        }

        // GET: api/Users/5
        [ResponseType(typeof(ProfileListing))]
        public IHttpActionResult GetUser(int id)
        {
            var request = new GetAllByPredicateQuery<User>
            {
                Predicate = u => u.Id == id
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
        public IHttpActionResult PostUser(CreateUserCommand command)
        {
            var user = _mediator.Send(command);
            return CreatedAtRoute("DefaultApi", new { id = user.Id }, user);
        }


        // PUT: api/Users/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutUser(long id, UpdateUserCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest();
            }

            _mediator.Send(command);
            return StatusCode(HttpStatusCode.NoContent);
        }

        // DELETE: api/Users/5
        [ResponseType(typeof(User))]
        public IHttpActionResult DeleteUser(long id)
        {
            var user = _mediator.Send(new DeleteUserCommand {Id = id});
            return Ok(user);
        }

        // POST: api/Users/1234/Add-Gym
        [ResponseType(typeof(User))]
        [HttpPost]
        [Route("api/Users/Add-Gym")]
        public IHttpActionResult AddUserToGym(AddUserToGymCommand command)
        {
            if (CurrentUser == null)
            {
                return Unauthorized();
            }

            command.UserId = CurrentUser.Id;
            var user = _mediator.Send(command);
            return Ok(MappingConfig.Instance.Map<ProfileListing>(user));
        }
    }
}