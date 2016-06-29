using System.Data.Entity;
using System.Linq;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Interfaces;
using justinobney.gymbuddy.api.Notifications;
using justinobney.gymbuddy.api.Requests.Decorators;
using MediatR;

namespace justinobney.gymbuddy.api.Requests.Appointments
{
    public class AppointmentInviteFriendCommand : IRequest
    {
        public long AppointmentId { get; set; }
        public long UserId { get; set; }
        public long FriendId { get; set; }
    }

    [DoNotValidate]
    public class AppointmentInviteFriendCommandHandler : RequestHandler<AppointmentInviteFriendCommand>
    {
        private readonly IPushNotifier _pushNotifier;
        private readonly IDbSet<User> _users;

        public AppointmentInviteFriendCommandHandler(IPushNotifier pushNotifier, IDbSet<User> users)
        {
            _pushNotifier = pushNotifier;
            _users = users;
        }

        protected override void HandleCore(AppointmentInviteFriendCommand message)
        {
            var inviter = _users
                .FirstOrDefault(x => x.Id == message.UserId);

            var friend = _users
                .Include(x => x.Devices)
                .FirstOrDefault(x => x.Id == message.FriendId);

            var additionalData = new AdditionalData
            {
                Type = NofiticationTypes.CreateAppointment,
                AppointmentId = message.AppointmentId
            };

            var payload = new NotificationPayload(additionalData)
            {
                Message = $"Gainz! {inviter.Name} has invited you to workout",
                Title = "LetMeLift"
            };

            _pushNotifier.Send(payload, friend.Devices.AsQueryable());
        }
    }
}