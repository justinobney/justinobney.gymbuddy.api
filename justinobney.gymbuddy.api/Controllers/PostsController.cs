using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using justinobney.gymbuddy.api.Data.AsyncJobs;
using justinobney.gymbuddy.api.Data.Posts;
using justinobney.gymbuddy.api.Requests.Generic;
using justinobney.gymbuddy.api.Requests.Posts;
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
            var post = _mediator.Send(new GetAllByPredicateQuery<Post>
            {
                Predicate = x => x.Id == id
            }).Include(x=>x.Contents).FirstOrDefault();

            //return Ok(MappingConfig.Instance.Map<PostListing>(Post));
            return Ok(post);
        }

        // POST: api/Post
        [ResponseType(typeof(AsyncJob))]
        public IHttpActionResult Post(CreatePostCommand post)
        {
            post.UserId = CurrentUser.Id;

            var job = _mediator.Send(post);
            return Ok(job);
        }
    }
}