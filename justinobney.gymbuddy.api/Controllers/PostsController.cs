using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
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
        
        [ResponseType(typeof(IEnumerable<Post>))]
        public async Task<IHttpActionResult> Get([FromUri] string lastId = "")
        {
            var result = await _mediator.SendAsync(new GetUserActivityQuery
            {
                UserId = CurrentUser.Id.ToString(),
                LastId = lastId
            });

            var postIds = result.Select(x => long.Parse(x.Object.Split(':').Last())).ToList();

            var posts = _mediator.Send(new GetAllByPredicateQuery<Post>
            {
                Predicate = post => postIds.Contains(post.Id)
            }).Include(x=>x.Contents);

            return Ok(posts);
        }

        // GET: api/Posts/{id}
        [ResponseType(typeof(Post))]
        public IHttpActionResult Get(long id)
        {
            var post = _mediator.Send(new GetAllByPredicateQuery<Post>
            {
                Predicate = x => x.Id == id
            }).Include(x=>x.Contents).FirstOrDefault();

            return Ok(post);
        }

        // POST: api/Posts
        [ResponseType(typeof(AsyncJob))]
        [HttpPost]
        public IHttpActionResult Post(CreatePostCommand post)
        {
            post.UserId = CurrentUser.Id;

            var job = _mediator.Send(post);
            return Ok(job);
        }
    }
}