using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using AutoMapper.QueryableExtensions;
using justinobney.gymbuddy.api.Data.AsyncJobs;
using justinobney.gymbuddy.api.Data.Posts;
using justinobney.gymbuddy.api.Requests.Generic;
using justinobney.gymbuddy.api.Requests.Posts;
using justinobney.gymbuddy.api.Responses;
using MediatR;
using Stream;

namespace justinobney.gymbuddy.api.Controllers
{
    public class PostsController : AuthenticatedController
    {
        public PostsController(IMediator mediator) : base(mediator)
        {
        }
        
        [ResponseType(typeof(TimelineActivityResponse))]
        public async Task<IHttpActionResult> Get([FromUri] string lastId = "")
        {
            var activity = await _mediator.SendAsync(new GetUserActivityQuery
            {
                UserId = CurrentUser.Id.ToString(),
                LastId = lastId
            });

            var activities = activity as Activity[] ?? activity.ToArray();

            var postIds = activities.Select(x => long.Parse(x.Object.Split(':').Last())).ToList();

            var posts = _mediator.Send(new GetAllByPredicateQuery<Post>
            {
                Predicate = post => postIds.Contains(post.Id)
            })
            .Include(x=>x.Contents)
            .ProjectTo<PostSummaryListing>(MappingConfig.Config);

            var result = new TimelineActivityResponse
            {
                Posts = posts.ToList(),
                Next = activities.LastOrDefault()?.Id
            };

            return Ok(result);
        }

        // GET: api/Posts/{id}
        [ResponseType(typeof(PostSummaryListing))]
        public IHttpActionResult Get(long id)
        {
            var post = _mediator.Send(new GetAllByPredicateQuery<Post>
            {
                Predicate = x => x.Id == id
            })
            .Include(x=>x.Contents)
            .ProjectTo<PostSummaryListing>(MappingConfig.Config)
            .FirstOrDefault();

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

    public class TimelineActivityResponse
    {
        public List<PostSummaryListing> Posts { get; set; }
        public string Next { get; set; }
    }
}