using Stream;

namespace justinobney.gymbuddy.api.Interfaces
{
    public interface IStreamClientProxy
    {
        void AddActivity(string feedSlug, string userId, Activity activity);
        void FollowFeed(string feedSlug, string userId, string targetFeedSlug, string targetUserId);
    }
}