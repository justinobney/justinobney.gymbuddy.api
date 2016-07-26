using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using justinobney.gymbuddy.api.Data.Posts;
using justinobney.gymbuddy.api.Enums;
using justinobney.gymbuddy.api.Interfaces;
using Stream;

namespace justinobney.gymbuddy.api.Helpers
{
    public class StreamConstants
    {
        public const string FeedUser = "user";
        public const string FeedTimeline = "timeline";
        public const string FeedTimelineAggregated = "timeline_agg";
    }

    public class StreamClientProxy : IStreamClientProxy
    {
        private readonly StreamClient _streamClient;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IDbSet<Post> _posts;

        public StreamClientProxy(
            StreamClient streamClient,
            IBackgroundJobClient backgroundJobClient,
            IDbSet<Post> posts
        )
        {
            _streamClient = streamClient;
            _backgroundJobClient = backgroundJobClient;
            _posts = posts;
        }
        
        public void FollowFeed(string feedSlug, string userId, string targetFeedSlug, string targetUserId)
        {
            _backgroundJobClient.Enqueue(() => _FollowFeed(feedSlug, userId, targetFeedSlug, targetUserId));
        }
        
        public void _FollowFeed(string feedSlug, string userId, string targetFeedSlug, string targetUserId)
        {
            var feed = _streamClient.Feed(feedSlug, userId);
            feed.FollowFeed(targetFeedSlug, targetUserId);
        }

        public async Task<IEnumerable<Activity>> GetActivity(string userId, string lastId)
        {
            var feed = _streamClient.Feed(StreamConstants.FeedTimeline, userId);
            IEnumerable<Activity> result = new List<Activity>();

            if (string.IsNullOrEmpty(lastId))
            {
                result = await feed.GetActivities();
            }
            else
            {
                result = await feed.GetActivities(0, 20, FeedFilter.Where().IdLessThan(lastId));
            }
            
            return result;
        }

        public void AddActivityFromPost(long postId)
        {
            _backgroundJobClient.Enqueue(() => AddActivityFromPostBackground(postId));
        }

        public void AddActivityFromPostBackground(long postId)
        {
            var post = _posts
                .Include(x => x.Contents)
                .FirstOrDefault(x => x.Id == postId);

            var activity = new Activity($"User:{post.UserId}", "post", $"Post:{post.Id}")
            {
                ForeignId = $"Post:{post.Id}"
            };

            var postActivity = new Dictionary<string, object>();

            var textContent = post.Contents.FirstOrDefault(x => x.Type == PostType.Text);
            if (textContent != null)
            {
                postActivity["text"] = textContent.Value;
            }

            var imageContent = post.Contents.FirstOrDefault(x => x.Type == PostType.Image);
            if (imageContent != null)
            {
                postActivity["imageUrl"] = imageContent.Value;
            }

            activity.SetData("post", postActivity);

            var feed = _streamClient.Feed(StreamConstants.FeedUser, $"{post.UserId}");
            Task.Run(() => feed.AddActivity(activity));
        }
    }
}