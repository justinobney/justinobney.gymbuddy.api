using System.Data.Entity;
using System.Linq;
using justinobney.gymbuddy.api.Data.Appointments;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Interfaces;
using justinobney.gymbuddy.api.Notifications;

namespace justinobney.gymbuddy.api.Requests.Appointments.Comments
{
    public class AppointmentAddCommentPushNotifier : IPostRequestHandler<AppointmentAddCommentCommand, Appointment>
    {
        private readonly IDbSet<User> _users;
        private readonly OtherPartiesNotifier _otherPartiesNotifier;

        public AppointmentAddCommentPushNotifier(IDbSet<User> users, OtherPartiesNotifier otherPartiesNotifier)
        {
            _users = users;
            _otherPartiesNotifier = otherPartiesNotifier;
        }

        public void Notify(AppointmentAddCommentCommand request, Appointment response)
        {
            var notifier = _users.First(x => x.Id == request.UserId);

            var additionalData = new AdditionalData { Type = NofiticationTypes.AddComment };
            var message = new NotificationPayload(additionalData)
            {
                Title = "GymSquad",
                Alert = $"{notifier.Name} posted a comment"
            };

            var notifierRequest = new OtherPartiesNotifierRequest
            {
                AppointmentId = request.AppointmentId,
                UserId = request.UserId,
                AdditionalData = additionalData
            };
            _otherPartiesNotifier.Send(notifierRequest, message);
        }
    }
}