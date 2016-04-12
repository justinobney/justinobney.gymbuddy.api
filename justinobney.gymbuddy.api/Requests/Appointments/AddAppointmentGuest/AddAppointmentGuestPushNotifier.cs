using System.Data.Entity;
using System.Linq;
using justinobney.gymbuddy.api.Data.Appointments;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Interfaces;
using justinobney.gymbuddy.api.Notifications;

namespace justinobney.gymbuddy.api.Requests.Appointments.AddAppointmentGuest
{
    public class AddAppointmentGuestPushNotifier : IPostRequestHandler<AddAppointmentGuestCommand, Appointment>
    {
        private readonly IDbSet<Appointment> _appointments;
        private readonly IDbSet<User> _users;
        private readonly PushNotifier _pushNotifier;

        public AddAppointmentGuestPushNotifier(IDbSet<Appointment> appointments, IDbSet<User> users, PushNotifier pushNotifier)
        {
            _appointments = appointments;
            _users = users;
            _pushNotifier = pushNotifier;
        }

        public void Notify(AddAppointmentGuestCommand request, Appointment response)
        {
            var appt = _appointments.First(x => x.Id == request.AppointmentId);
            var apptOwner = _users
                .Include(x=>x.Devices)
                .First(x => x.Id == appt.UserId);

            var guest = _users.First(x => x.Id == request.UserId);

            var additionalData = new AdditionalData { Type = NofiticationTypes.AddAppointmentGuest };
            var message = new NotificationPayload(additionalData)
            {
                Alert = $"{guest.Name} wants to work in",
                Title = "Appointment Guest Request"
            };
            
            _pushNotifier.Send(message, apptOwner.Devices.AsQueryable());
        }

    }
}