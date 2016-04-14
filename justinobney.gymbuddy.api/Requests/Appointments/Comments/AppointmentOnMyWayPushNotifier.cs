using System.Linq;
using justinobney.gymbuddy.api.Data.Appointments;
using justinobney.gymbuddy.api.Enums;
using justinobney.gymbuddy.api.Interfaces;
using justinobney.gymbuddy.api.Notifications;

namespace justinobney.gymbuddy.api.Requests.Appointments.Comments
{
    public class AppointmentOnMyWayPushNotifier : IPostRequestHandler<AppointmentOnMyWayCommand, Appointment>
    {
        private readonly PushNotifier _pushNotifier;

        public AppointmentOnMyWayPushNotifier(PushNotifier pushNotifier)
        {
            _pushNotifier = pushNotifier;
        }
        
        public void Notify(AppointmentOnMyWayCommand request, Appointment response)
        {
            var additionalData = new AdditionalData { Type = NofiticationTypes.AppointmentOnMyWay };
            var message = new NotificationPayload(additionalData)
            {
                Title = "On My Way"
            };

            var guestDevices = response.GuestList
                .Where(x=>x.Status == AppointmentGuestStatus.Confirmed)
                .SelectMany(x => x.User.Devices)
                .AsQueryable();
            
            var devices = response.User.Devices.Concat(guestDevices)
                .Where(x=>x.UserId != request.UserId)
                .AsQueryable();

            _pushNotifier.Send(message, devices);
        }
    }
}