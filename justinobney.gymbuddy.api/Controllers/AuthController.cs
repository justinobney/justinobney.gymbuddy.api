using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Http;
using System.Web.Http.Description;
using AutoMapper.QueryableExtensions;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Requests.Generic;
using justinobney.gymbuddy.api.Responses;
using MediatR;

namespace justinobney.gymbuddy.api.Controllers
{
    public class AuthController : AuthenticatedController
    {
        public AuthController(IMediator mediator) : base(mediator)
        {
        }

        // GET: api/Auth
        [ResponseType(typeof(ProfileListing))]
        public IHttpActionResult Get()
        {
            if (Request.Headers.Contains("device-id"))
            {
                var deviceId = Request.Headers.GetValues("device-id").FirstOrDefault();
                Expression<Func<User, bool>> predicate = u => u.Devices.Any(d => d.DeviceId == deviceId);
                var request = new GetAllByPredicateQuery<User> {Predicate = predicate};

                var user = _mediator.Send(request)
                    .Include(x => x.Gyms)
                    .ProjectTo<ProfileListing>(MappingConfig.Config)
                    .FirstOrDefault();

                return Ok(user);
            }
            return Ok();
        }
    }
}