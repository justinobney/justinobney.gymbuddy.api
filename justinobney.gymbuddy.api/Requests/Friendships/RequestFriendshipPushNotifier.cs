using System.Data.Entity;
using System.Linq;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Interfaces;
using justinobney.gymbuddy.api.Notifications;

namespace justinobney.gymbuddy.api.Requests.Friendships
{
    public class RequestFriendshipPushNotifier : IPostRequestHandler<RequestFriendshipCommand, Friendship>
    {
        private readonly IDbSet<User> _users;
        private readonly IPushNotifier _pushNotifier;

        public RequestFriendshipPushNotifier(
            IDbSet<User> users,
            IPushNotifier pushNotifier
            )
        {
            _users = users;
            _pushNotifier = pushNotifier;
        }
        
        public void Notify(RequestFriendshipCommand request, Friendship response)
        {
            var friend = _users
                .Include(x => x.Devices)
                .First(x => x.Id == response.FriendId);

            var initiator = _users
                .Include(x => x.Devices)
                .First(x => x.Id == response.UserId);

            var additionalData = new AdditionalData { Type = NofiticationTypes.RequestFriendship };
                var message = new NotificationPayload(additionalData)
                {
                    Alert = $"{initiator.Name} wants to join the squad",
                    Title = "Squad Up"
                };

            _pushNotifier.Send(message, friend.Devices.AsQueryable());
        }
    }
}