using System.Data.Entity;
using System.Linq;
using justinobney.gymbuddy.api.Data.Appointments;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Interfaces;
using justinobney.gymbuddy.api.Notifications;
using RestSharp;

namespace justinobney.gymbuddy.api.Requests.Appointments.RemoveAppointmentGuest
{
    public class RemoveAppointmentGuestPushNotifier : IPostRequestHandler<RemoveAppointmentGuestCommand, Appointment>
    {
        private readonly IDbSet<Appointment> _appointments;
        private readonly IDbSet<User> _users;
        private readonly IRestClient _client;

        public RemoveAppointmentGuestPushNotifier(IDbSet<Appointment> appointments, IDbSet<User> users, IRestClient client)
        {
            _appointments = appointments;
            _users = users;
            _client = client;
        }

        public void Notify(RemoveAppointmentGuestCommand request, Appointment response)
        {
            var appt = _appointments
                .First(x => x.Id == request.AppointmentId);

            var apptOwner = _users
                .Include(x => x.Devices)
                .First(x => x.Id == appt.UserId);

            var guest = _users.First(x => x.Id == request.UserId);

            var additionalData = new AdditionalData { Type = NofiticationTypes.RemoveAppointmentGuest };
            var message = new NotificationPayload(additionalData)
            {
                Alert = $"{guest.Name} left your plans",
                Title = "Appointment Guest Left :("
            };

            var iosNotification = new IonicPushNotification(message)
            {
                Production = true,
                Tokens = apptOwner.Devices
                    .Where(y => y.Platform == "iOS" && !string.IsNullOrEmpty(y.PushToken))
                    .Select(y => y.PushToken)
                    .ToList()
            };

            var androidNotification = new IonicPushNotification(message)
            {
                Tokens = apptOwner.Devices
                    .Where(y => y.Platform == "Android" && !string.IsNullOrEmpty(y.PushToken))
                    .Select(y => y.PushToken)
                    .ToList()
            };

            iosNotification.Send(_client);
            androidNotification.Send(_client);
        }

    }
}