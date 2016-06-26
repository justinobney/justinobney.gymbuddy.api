using System.Data.Entity;
using System.Linq;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Interfaces;
using justinobney.gymbuddy.api.Notifications;

namespace justinobney.gymbuddy.api.Requests.Friendships
{
    public class ConfirmFriendshipPushNotifier : IPostRequestHandler<ConfirmFriendshipCommand, Friendship>
    {
        private readonly IDbSet<User> _users;
        private readonly IPushNotifier _pushNotifier;

        public ConfirmFriendshipPushNotifier(
            IDbSet<User> users,
            IPushNotifier pushNotifier
            )
        {
            _users = users;
            _pushNotifier = pushNotifier;
        }

        public void Notify(ConfirmFriendshipCommand request, Friendship response)
        {
            var initiator = _users
                .Include(x => x.Devices)
                .First(x => x.Id == response.FriendId);

            var friend = _users
                .Include(x => x.Devices)
                .First(x => x.Id == response.UserId);

            var additionalData = new AdditionalData { Type = NofiticationTypes.RequestFriendship };
            var message = new NotificationPayload(additionalData)
            {
                Message = $"{friend.Name} added you to the squad",
                Title = "Squad Up"
            };

            _pushNotifier.Send(message, initiator.Devices.AsQueryable());
        }
    }
}