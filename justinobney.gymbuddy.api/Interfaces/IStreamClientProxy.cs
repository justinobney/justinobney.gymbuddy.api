using System.Collections.Generic;
using System.Threading.Tasks;
using Stream;

namespace justinobney.gymbuddy.api.Interfaces
{
    public interface IStreamClientProxy
    {
        void FollowFeed(string feedSlug, string userId, string targetFeedSlug, string targetUserId);

        Task<IEnumerable<Activity>> GetActivity(string userId, string lastId);

        void AddActivityFromPost(long postId);

        string GetTimelineToken(string userId);
    }
}