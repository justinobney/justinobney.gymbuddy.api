using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using AutoMapper.QueryableExtensions;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Requests.Friendships;
using justinobney.gymbuddy.api.Responses;
using MediatR;

namespace justinobney.gymbuddy.api.Controllers
{
    public class FriendshipController : AuthenticatedController
    {

        public FriendshipController(IMediator mediator) : base(mediator)
        {

        }

        // GET: api/Friendship/Requests
        [Route("api/Friendship/Requests")]
        [ResponseType(typeof(IEnumerable<FriendshipListing>))]
        public IHttpActionResult GetRequests()
        {
            var friendship = _mediator.Send(new GetAllFriendshipRequestsQuery
            {
                UserId = CurrentUser.Id
            })
            .ProjectTo<FriendshipListing>(MappingConfig.Config);

            return Ok(friendship);
        }

        [Route("api/Friendship")]
        [ResponseType(typeof(IEnumerable<FriendshipListing>))]
        public IHttpActionResult Get()
        {
            var friendship = _mediator.Send(new GetAllFriendshipsQuery
            {
                UserId = CurrentUser.Id
            })
            .Select(x=>x.Friend)
            .ProjectTo<ProfileListing>(MappingConfig.Config);

            return Ok(friendship);
        }

        // GET: api/Friendship/{id}
        [ResponseType(typeof(FriendshipListing))]
        public IHttpActionResult Get(long id)
        {
            var friendship = _mediator.Send(new GetFriendshipQuery
            {
                UserId = CurrentUser.Id,
                FriendId = id
            });

            return Ok(MappingConfig.Instance.Map<FriendshipListing>(friendship));
        }

        // POST: api/Friendship/{id}
        [ResponseType(typeof(FriendshipListing))]
        public IHttpActionResult Post(long id)
        {
            var friendship = _mediator.Send(new RequestFriendshipCommand
            {
                UserId = CurrentUser.Id,
                FriendId = id
            });

            return Ok(MappingConfig.Instance.Map<FriendshipListing>(friendship));
        }

        // POST: api/Friendship/{id}/Confirm
        [ResponseType(typeof(FriendshipListing))]
        [Route("api/Friendship/{id}/Confirm")]
        [HttpPost]
        public IHttpActionResult ConfirmFriendship(long id)
        {
            var friendship = _mediator.Send(new ConfirmFriendshipCommand
            {
                UserId = CurrentUser.Id,
                FriendId = id
            });

            return Ok(MappingConfig.Instance.Map<FriendshipListing>(friendship));
        }
    }
}