using System.Data.Entity;
using System.Linq;
using justinobney.gymbuddy.api.Data.Appointments;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Enums;
using justinobney.gymbuddy.api.Interfaces;
using justinobney.gymbuddy.api.Notifications;

namespace justinobney.gymbuddy.api.Requests.Appointments.Comments
{
    public class AppointmentOnMyWayPushNotifier : IPostRequestHandler<AppointmentOnMyWayCommand, Appointment>
    {
        private readonly PushNotifier _pushNotifier;
        private readonly IDbSet<Appointment> _appointments;
        private readonly IDbSet<AppointmentGuest> _guests;
        private readonly IDbSet<User> _users;

        public AppointmentOnMyWayPushNotifier(PushNotifier pushNotifier, IDbSet<Appointment> appointments , IDbSet<AppointmentGuest> guests, IDbSet<User> users)
        {
            _pushNotifier = pushNotifier;
            _appointments = appointments;
            _guests = guests;
            _users = users;
        }

        public void Notify(AppointmentOnMyWayCommand request, Appointment response)
        {
            var notifier = _users.First(x=>x.Id == request.UserId);

            var additionalData = new AdditionalData { Type = NofiticationTypes.AppointmentOnMyWay };
            var message = new NotificationPayload(additionalData)
            {
                Title = "GymSquad",
                Alert = $"{notifier.Name} is on the way to the gym"
            };
            
            var guestDevices = _guests
                .Include(x => x.User.Devices)
                .Where(x => x.AppointmentId == request.AppointmentId)
                .Where(x => x.Status == AppointmentGuestStatus.Confirmed)
                .SelectMany(x => x.User.Devices)
                .AsQueryable();

            var devices = _appointments
                .Include(x => x.User.Devices)
                .First(x => x.Id == request.AppointmentId)
                .User.Devices
                .Concat(guestDevices)
                .Where(x=>x.UserId != request.UserId)
                .AsQueryable();

            _pushNotifier.Send(message, devices);
        }
    }
}