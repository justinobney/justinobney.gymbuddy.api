using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using Hangfire;
using justinobney.gymbuddy.api.Data.Posts;
using justinobney.gymbuddy.api.Enums;
using justinobney.gymbuddy.api.Interfaces;
using justinobney.gymbuddy.api.Responses;
using Stream;

namespace justinobney.gymbuddy.api.Helpers
{
    public class StreamConstants
    {
        public const string FeedUser = "user";
        public const string FeedUserPosts = "user_posts";
        public const string FeedTimeline = "timeline";
        public const string FeedNotifications = "notifications";
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

        // This is not covered by a test because of issues with
        // NSubstitute mocking the internal constructor...
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

        public string GetTimelineToken(string userId)
        {
            return _streamClient.Feed(StreamConstants.FeedTimeline, userId).Token;
        }

        public void AddActivityFromPostBackground(long postId)
        {
            var post = _posts
                .ProjectTo<PostSummaryListing>(MappingConfig.Config)
                .FirstOrDefault(x => x.Id == postId);

            var activity = CreateActivityFromPost(post);

            var feed = _streamClient.Feed(StreamConstants.FeedUserPosts, $"{post.UserId}");
            Task.Run(() => feed.AddActivity(activity));
        }

        public Activity CreateActivityFromPost(PostSummaryListing postSummary)
        {
            var activity = new Activity($"User:{postSummary.UserId}", "post", $"Post:{postSummary.Id}")
            {
                ForeignId = $"Post:{postSummary.Id}"
            };
            
            activity.SetData("meta", postSummary);
            return activity;
        }
    }
}