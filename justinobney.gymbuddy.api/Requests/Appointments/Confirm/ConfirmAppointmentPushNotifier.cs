using System.Data.Entity;
using System.Linq;
using justinobney.gymbuddy.api.Data.Appointments;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Enums;
using justinobney.gymbuddy.api.Interfaces;
using justinobney.gymbuddy.api.Notifications;
using RestSharp;

namespace justinobney.gymbuddy.api.Requests.Appointments.Confirm
{
    public class ConfirmAppointmentPushNotifier : IPostRequestHandler<ConfirmAppointmentCommand, Appointment>
    {
        private readonly IDbSet<Appointment> _appointments;
        private readonly IDbSet<User> _users;
        private readonly IRestClient _client;

        public ConfirmAppointmentPushNotifier(IDbSet<Appointment> appointments, IDbSet<User> users, IRestClient client)
        {
            _appointments = appointments;
            _users = users;
            _client = client;
        }

        public void Notify(ConfirmAppointmentCommand request, Appointment response)
        {
            var appt = _appointments
                .Include(x=>x.User)
                .Include(x=>x.GuestList)
                .First(x => x.Id == request.AppointmentId);

            var approvedGuests = appt.GuestList
                .ToList()
                .Where(x => x.Status == AppointmentGuestStatus.Confirmed)
                .Select(x => x.UserId);

            var guests = _users
                .Include(x => x.Devices)
                .Where(x => approvedGuests.Contains(x.Id));

            var additionalData = new AdditionalData { Type = NofiticationTypes.ConfirmAppointment };
            var message = new NotificationPayload(additionalData)
            {
                Alert = $"{appt.User.Name} confirmed.",
                Title = "Workout Session Confirmed"
            };

            var iosNotification = new IonicPushNotification(message)
            {
                Production = true,
                Tokens = guests.SelectMany(x => x.Devices
                    .Where(y => y.Platform == "iOS" && !string.IsNullOrEmpty(y.PushToken))
                    .Select(y => y.PushToken))
                    .ToList()
            };

            var androidNotification = new IonicPushNotification(message)
            {
                Tokens = guests.SelectMany(x => x.Devices
                    .Where(y => y.Platform == "Android" && !string.IsNullOrEmpty(y.PushToken))
                    .Select(y => y.PushToken))
                    .ToList()
            };

            iosNotification.Send(_client);
            androidNotification.Send(_client);
        }

    }
}