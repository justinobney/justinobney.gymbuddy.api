using AutoMapper.QueryableExtensions;
using justinobney.gymbuddy.api.Requests.Friendships;
using justinobney.gymbuddy.api.Responses;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;

namespace justinobney.gymbuddy.api.Controllers
{
    public class FriendshipController : AuthenticatedController
    {

        public FriendshipController(IMediator mediator) : base(mediator)
        {

        }

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
            .Select(x => x.Friend)
            .ProjectTo<ProfileListing>(MappingConfig.Config);

            return Ok(friendship);
        }

        [Route("api/Friendship/{friendId}")]
        [ResponseType(typeof(FriendshipListing))]
        public IHttpActionResult Get(long friendId)
        {
            var friendship = _mediator.Send(new GetFriendshipQuery
            {
                UserId = CurrentUser.Id,
                FriendId = friendId
            });

            return Ok(MappingConfig.Instance.Map<FriendshipListing>(friendship));
        }

        [HttpPost]
        [Route("api/Friendship/{friendId}")]
        [ResponseType(typeof(FriendshipListing))]
        public IHttpActionResult Post(long friendId)
        {
            var friendship = _mediator.Send(new RequestFriendshipCommand
            {
                UserId = CurrentUser.Id,
                FriendId = friendId
            });

            return Ok(MappingConfig.Instance.Map<FriendshipListing>(friendship));
        }

        [HttpPost]
        [Route("api/Friendship/{friendId}/Confirm")]
        [ResponseType(typeof(FriendshipListing))]
        public IHttpActionResult ConfirmFriendship(long friendId)
        {
            var friendship = _mediator.Send(new ConfirmFriendshipCommand
            {
                UserId = CurrentUser.Id,
                FriendId = friendId
            });

            return Ok(MappingConfig.Instance.Map<FriendshipListing>(friendship));
        }

        [ResponseType(typeof(ICollection<FacebookFriendshipListing>))]
        [HttpPost]
        [Route("api/Friendship/Get-By-Facebook-Ids")]
        public IHttpActionResult GetByFacebookIds(ICollection<string> fbIds)
        {
            if (fbIds == null || !fbIds.Any())
                return Content((HttpStatusCode)422, "Unprocessable Entity: Invalid parameter(s).");

            return Ok(_mediator.Send(new GetFriendshipsByFacebookIdsQuery { UserId = CurrentUser.Id, FbIds = fbIds }));
        }
    }
}