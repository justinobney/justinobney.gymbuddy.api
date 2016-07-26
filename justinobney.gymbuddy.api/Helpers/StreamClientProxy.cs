using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
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

        public StreamClientProxy(
            StreamClient streamClient,
            IBackgroundJobClient backgroundJobClient
        )
        {
            _streamClient = streamClient;
            _backgroundJobClient = backgroundJobClient;
        }

        public void AddActivity(string feedSlug, string userId, Activity activity)
        {
            _backgroundJobClient.Enqueue(() => _AddActivity(feedSlug, userId, activity));
        }

        public void _AddActivity(string feedSlug, string userId, Activity activity)
        {
            var feed = _streamClient.Feed(feedSlug, userId);
            Task.Run(() => feed.AddActivity(activity));
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
    }
}