using System.Data.Entity;
using System.Linq;
using justinobney.gymbuddy.api.Data.Appointments;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Interfaces;
using justinobney.gymbuddy.api.Notifications;

namespace justinobney.gymbuddy.api.Requests.Appointments.Kudos
{
    public class AppointmentToggleKudosPushNotifier : IPostRequestHandler<AppointmentToggleKudosCommand, Appointment>
    {
        private readonly IPushNotifier _pushNotifier;
        private readonly IDbSet<User> _users;
        private readonly IDbSet<AppointmentKudos> _appointmentKudos;

        public AppointmentToggleKudosPushNotifier(
            IPushNotifier pushNotifier,
            IDbSet<User> users,
            IDbSet<AppointmentKudos> appointmentKudos 
            )
        {
            _pushNotifier = pushNotifier;
            _users = users;
            _appointmentKudos = appointmentKudos;
        }

        public void Notify(AppointmentToggleKudosCommand request, Appointment response)
        {
            var userRevokedKudos = !_appointmentKudos.Any(x => x.UserId == request.UserId && x.AppointmentId == response.Id);
            
            var friend = _users.FirstOrDefault(x=>x.Id == request.UserId);

            var workoutHost = _users
                .Include(x=>x.Devices)
                .FirstOrDefault(x=>x.Id == response.UserId);

            if (userRevokedKudos || (friend.Id == workoutHost.Id))
                return;

            var additionalData = new AdditionalData
            {
                Type = NofiticationTypes.AppointmentKudos,
                AppointmentId = response.Id
            };

            var message = new NotificationPayload(additionalData)
            {
                Message = $"Kudos: {friend.Name} approves",
                Title = "Kudos"
            };

            _pushNotifier.Send(message, workoutHost.Devices.AsQueryable());
        }
    }
}