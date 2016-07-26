using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using justinobney.gymbuddy.api.Data;
using justinobney.gymbuddy.api.Data.AsyncJobs;
using justinobney.gymbuddy.api.Data.Posts;
using justinobney.gymbuddy.api.Enums;
using justinobney.gymbuddy.api.Helpers;
using justinobney.gymbuddy.api.Interfaces;
using Stream;

namespace justinobney.gymbuddy.api.Requests.Posts
{
    public class TextContentStrategy: IPostContentStategy
    {
        private readonly IDbSet<Post> _posts;
        private readonly IStreamClientProxy _streamClientProxy;
        private readonly AppContext _context;

        public TextContentStrategy(
            IDbSet<Post> posts,
            IStreamClientProxy streamClientProxy,
            AppContext context
            )
        {
            _posts = posts;
            _context = context;
            _streamClientProxy = streamClientProxy;
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
            
            AddToStream(post);

            var job = new AsyncJob
            {
                Status = JobStatus.Complete,
                ContentUrl = $"/api/posts/{post.Id}", //TODO: make URL factory
            };

            return job;
        }

        private void AddToStream(Post post)
        {
            var activity = new Activity($"User:{post.UserId}", "post", $"Post:{post.Id}")
            {
                ForeignId = $"Post:{post.Id}"
            };

            var postActivity = new Dictionary<string, object>();
            postActivity["text"] = post.Contents.First().Value;
            activity.SetData("post", postActivity);

            _streamClientProxy.AddActivity(StreamConstants.FeedUser, $"{post.UserId}", activity);
        }
    }
}