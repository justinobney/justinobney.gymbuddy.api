using System.Data.Entity;
using System.Linq;
using justinobney.gymbuddy.api.Data.Appointments;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Interfaces;
using justinobney.gymbuddy.api.Notifications;
using RestSharp;

namespace justinobney.gymbuddy.api.Requests.Appointments.Delete
{
    public class DeleteAppointmentPushNotifier : IPostRequestHandler<DeleteAppointmentCommand, Appointment>
    {

        private readonly IRestClient _client;

        public DeleteAppointmentPushNotifier(IRestClient client)
        {
            _client = client;
        }

        public void Notify(DeleteAppointmentCommand request, Appointment response)
        {
            
            var additionalData = new AdditionalData { Type = NofiticationTypes.CancelAppointment };
            var message = new NotificationPayload(additionalData)
            {
                Alert = request.NotificaitonAlert,
                Title = request.NotificaitonTitle
            };

            var iosNotification = new IonicPushNotification(message)
            {
                Production = true,
                Tokens = request.Guests.SelectMany(x => x.Devices
                    .Where(y => y.Platform == "iOS" && !string.IsNullOrEmpty(y.PushToken))
                    .Select(y => y.PushToken))
                    .ToList()
            };

            var androidNotification = new IonicPushNotification(message)
            {
                Tokens = request.Guests.SelectMany(x => x.Devices
                    .Where(y => y.Platform == "Android" && !string.IsNullOrEmpty(y.PushToken))
                    .Select(y => y.PushToken))
                    .ToList()
            };

            iosNotification.Send(_client);
            androidNotification.Send(_client);
        }

    }
}