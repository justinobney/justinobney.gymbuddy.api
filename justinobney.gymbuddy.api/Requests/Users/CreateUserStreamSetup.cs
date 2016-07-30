using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Helpers;
using justinobney.gymbuddy.api.Interfaces;

namespace justinobney.gymbuddy.api.Requests.Users
{
    public class CreateUserStreamSetup : IPostRequestHandler<CreateUserCommand, User>
    {
        private readonly IStreamClientProxy _streamClientProxy;

        public CreateUserStreamSetup(
            IStreamClientProxy streamClientProxy
        )
        {
            _streamClientProxy = streamClientProxy;
        }


        public void Notify(CreateUserCommand request, User response)
        {
            _streamClientProxy.FollowFeed(StreamConstants.FeedTimeline, $"{response.Id}", StreamConstants.FeedUserPosts, $"{response.Id}");
        }
    }
}