using System.Data.Entity;
using System.Linq;
using justinobney.gymbuddy.api.Data.Appointments;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Interfaces;
using justinobney.gymbuddy.api.Notifications;

namespace justinobney.gymbuddy.api.Requests.Appointments.Create
{
    public class CreateAppointmentNotifier : IPostRequestHandler<CreateAppointmentCommand, Appointment>
    {
        private readonly IDbSet<User> _users;
        private readonly IDbSet<Friendship> _friendships;
        private readonly IPushNotifier _pushNotifier;

        public CreateAppointmentNotifier(
            IDbSet<User> users,
            IDbSet<Friendship> friendships,
            IPushNotifier pushNotifier
            )
        {
            _users = users;
            _friendships = friendships;
            _pushNotifier = pushNotifier;
        }

        public void Notify(CreateAppointmentCommand request, Appointment response)
        {
            var friendUserIds = _friendships.Where(x => x.UserId == request.UserId && x.Status == Enums.FriendshipStatus.Active).Select(x => x.FriendId).ToList();
            var notifyUsers = _users
                .Include(x => x.Devices)
                .Include(x => x.Gyms)
                .Where(x => !x.SilenceAllNotifications && ((x.NewGymWorkoutNotifications && x.Gyms.Any(y => y.Id == request.GymId)) || (x.NewSquadWorkoutNotifications && friendUserIds.Contains(x.Id))) && x.Id != request.UserId)
                .ToList();

            var additionalData = new AdditionalData
            {
                Type = NofiticationTypes.CreateAppointment,
                AppointmentId = response.Id
            };

            var place = response.Location ?? response.Gym.Name;

            var message = new NotificationPayload(additionalData)
            {
                Title = $"{response.User.Name} wants to work out",
                Message = $"{request.Title} at {place}"
            };

            _pushNotifier.Send(message, notifyUsers.SelectMany(x => x.Devices));
        }
    }
}