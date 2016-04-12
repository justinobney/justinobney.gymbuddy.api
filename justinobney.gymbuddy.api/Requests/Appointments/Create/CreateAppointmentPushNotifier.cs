using System.Data.Entity;
using System.Linq;
using justinobney.gymbuddy.api.Data.Appointments;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Interfaces;
using justinobney.gymbuddy.api.Notifications;

namespace justinobney.gymbuddy.api.Requests.Appointments.Create
{
    public class CreateAppointmentPushNotifier : IPostRequestHandler<CreateAppointmentCommand, Appointment>
    {
        private readonly IDbSet<User> _users;
        private readonly PushNotifier _pushNotifier;

        public CreateAppointmentPushNotifier(IDbSet<User> users , PushNotifier pushNotifier)
        {
            _users = users;
            _pushNotifier = pushNotifier;
        }

        public void Notify(CreateAppointmentCommand request, Appointment response)
        {
            var notifyUsers = _users
                .Include(x => x.Devices)
                .Include(x => x.Gyms)
                .Where(x => x.Gyms.Any(y => y.Id == request.GymId))
                .Where(x => x.Id != request.UserId);

            var additionalData = new AdditionalData {Type = NofiticationTypes.CreateAppointment };
            var message = new NotificationPayload(additionalData)
            {
                Alert = $"{response.User.Name} wants to work: {request.Title}",
                Title = "New Appointment Available"
            };

            _pushNotifier.Send(message, notifyUsers.SelectMany(x => x.Devices));
        }
        
    }
}