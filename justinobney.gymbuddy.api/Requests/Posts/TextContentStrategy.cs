using System.Data.Entity;
using justinobney.gymbuddy.api.Data;
using justinobney.gymbuddy.api.Data.AsyncJobs;
using justinobney.gymbuddy.api.Data.Posts;
using justinobney.gymbuddy.api.Enums;
using justinobney.gymbuddy.api.Interfaces;

namespace justinobney.gymbuddy.api.Requests.Posts
{
    public class TextContentStrategy: IPostContentStategy
    {
        private readonly IDbSet<Post> _posts;
        private readonly AppContext _context;

        public TextContentStrategy(
            IDbSet<Post> posts,
            AppContext context
            )
        {
            _posts = posts;
            _context = context;
        }

        public AsyncJob Handle(CreatePostCommand message)
        {
            var post = new Post
            {
                Contents = message.Content,
                UserId = message.UserId
            };

            _posts.Add(post);
            _context.SaveChanges();

            var job = new AsyncJob
            {
                Id = 0,
                Status = JobStatus.Complete,
                StatusUrl = null,
                ContentUrl = $"/api/posts/{post.Id}", //TODO: make URL factory
            };

            return job;
        }
    }
}