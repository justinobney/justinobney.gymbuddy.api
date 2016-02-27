using System.Data.Entity;
using System.Linq;
using justinobney.gymbuddy.api.Data.Appointments;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Interfaces;
using justinobney.gymbuddy.api.Notifications;
using RestSharp;

namespace justinobney.gymbuddy.api.Requests.Appointments.Create
{
    public class CreateAppointmentPushNotifier : IPostRequestHandler<CreateAppointmentCommand, Appointment>
    {
        private readonly IDbSet<User> _users;
        private readonly IRestClient _client;

        public CreateAppointmentPushNotifier(IDbSet<User> users , IRestClient client)
        {
            _users = users;
            _client = client;
        }

        public void Notify(CreateAppointmentCommand request, Appointment response)
        {
            var notifyUsers = _users
                .Include(x => x.Devices)
                .Include(x => x.Gyms)
                .Where(x => x.Gyms.Any(y => y.Id == request.GymId))
                .Where(x => x.Id != request.UserId);

            var additionalData = new AdditionalData {Type = "CreateAppointment" };
            var message = new NotificationPayload(additionalData)
            {
                Alert = $"{response.User.Name} wants to work: {request.Description}",
                Title = "New Appointment Available"
            };

            var iosNotification = new IonicPushNotification(message)
            {
                Production = true,
                Tokens = notifyUsers.SelectMany(x => x.Devices
                    .Where(y => y.Platform == "iOS" && !string.IsNullOrEmpty(y.PushToken))
                    .Select(y => y.PushToken))
                    .ToList()
            };

            var androidNotification = new IonicPushNotification(message)
            {
                Tokens = notifyUsers.SelectMany(x => x.Devices
                    .Where(y => y.Platform == "Android" && !string.IsNullOrEmpty(y.PushToken))
                    .Select(y => y.PushToken))
                    .ToList()
            };

            iosNotification.Send(_client);
            androidNotification.Send(_client);
        }
        
    }
}