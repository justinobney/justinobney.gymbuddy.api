using System.Data.Entity;
using System.Linq;
using justinobney.gymbuddy.api.Data.Appointments;
using justinobney.gymbuddy.api.Interfaces;
using justinobney.gymbuddy.api.Notifications;
using RestSharp;

namespace justinobney.gymbuddy.api.Requests.Appointments.Confirm
{
    public class ConfirmAppointmentGuestPushNotifier : IPostRequestHandler<ConfirmAppointmentGuestCommand, AppointmentGuest>
    {
        private readonly IDbSet<Appointment> _appointments;
        private readonly IDbSet<AppointmentGuest> _guests;
        private readonly IRestClient _client;

        public ConfirmAppointmentGuestPushNotifier(IDbSet<Appointment> appointments, IDbSet<AppointmentGuest> guests, IRestClient client)
        {
            _appointments = appointments;
            _guests = guests;
            _client = client;
        }

        public void Notify(ConfirmAppointmentGuestCommand request, AppointmentGuest response)
        {
            var appt = _appointments
                .Include(x => x.User)
                .Include(x => x.GuestList)
                .First(x => x.Id == request.AppointmentId);

            var guest = _guests
                .Include(x=>x.User)
                .First(x => x.Id == request.AppointmentGuestId);
            
            var additionalData = new AdditionalData { Type = NofiticationTypes.ConfirmAppointmentGuest };
            var message = new NotificationPayload(additionalData)
            {
                Alert = $"{appt.User.Name} confirmed.",
                Title = "Workout Session Confirmed"
            };

            var iosNotification = new IonicPushNotification(message)
            {
                Production = true,
                Tokens = guest.User.Devices
                    .Where(y => y.Platform == "iOS" && !string.IsNullOrEmpty(y.PushToken))
                    .Select(y => y.PushToken)
                    .ToList()
            };

            var androidNotification = new IonicPushNotification(message)
            {
                Tokens = guest.User.Devices
                    .Where(y => y.Platform == "Android" && !string.IsNullOrEmpty(y.PushToken))
                    .Select(y => y.PushToken)
                    .ToList()
            };

            iosNotification.Send(_client);
            androidNotification.Send(_client);
        }

    }
}