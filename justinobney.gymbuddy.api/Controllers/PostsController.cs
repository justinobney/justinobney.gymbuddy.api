using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;
using justinobney.gymbuddy.api.Data.Posts;
using MediatR;

namespace justinobney.gymbuddy.api.Controllers
{
    public class PostsController : AuthenticatedController
    {
        public PostsController(IMediator mediator) : base(mediator)
        {
        }

        // GET: api/Post/Requests
        [Route("api/Post/Requests")]
        [ResponseType(typeof(IEnumerable<Post>))]
        public IHttpActionResult GetRequests()
        {
            //var Post = _mediator.Send(new GetAllPostRequestsQuery
            //{
            //    UserId = CurrentUser.Id
            //})
            //    .ProjectTo<PostListing>(MappingConfig.Config);

            return Ok("Not Implemented");
        }

        [Route("api/Post")]
        [ResponseType(typeof(IEnumerable<Post>))]
        public IHttpActionResult Get()
        {
            //var Post = _mediator.Send(new GetAllPostsQuery
            //{
            //    UserId = CurrentUser.Id
            //})
            //    .Select(x => x.Friend)
            //    .ProjectTo<ProfileListing>(MappingConfig.Config);

            return Ok("Not Implemented");
        }

        // GET: api/Post/{id}
        [ResponseType(typeof(Post))]
        public IHttpActionResult Get(long id)
        {
            //var Post = _mediator.Send(new GetPostQuery
            //{
            //    UserId = CurrentUser.Id,
            //    FriendId = id
            //});

            //return Ok(MappingConfig.Instance.Map<PostListing>(Post));
            return Ok("Not Implemented");
        }

        // POST: api/Post/{id}
        [ResponseType(typeof(Post))]
        public IHttpActionResult Post(long id)
        {
            return Ok("Not Implemented");
        }
    }
}