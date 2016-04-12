using System.Linq;
using justinobney.gymbuddy.api.Data.Appointments;
using justinobney.gymbuddy.api.Interfaces;
using justinobney.gymbuddy.api.Notifications;

namespace justinobney.gymbuddy.api.Requests.Appointments.Delete
{
    public class DeleteAppointmentPushNotifier : IPostRequestHandler<DeleteAppointmentCommand, Appointment>
    {
        private readonly PushNotifier _pushNotifier;

        public DeleteAppointmentPushNotifier(PushNotifier pushNotifier)
        {
            _pushNotifier = pushNotifier;
        }

        public void Notify(DeleteAppointmentCommand request, Appointment response)
        {
            
            var additionalData = new AdditionalData { Type = NofiticationTypes.CancelAppointment };
            var message = new NotificationPayload(additionalData)
            {
                Alert = request.NotificaitonAlert,
                Title = request.NotificaitonTitle
            };

            _pushNotifier.Send(message, request.Guests.SelectMany(x => x.Devices).AsQueryable());
        }

    }
}