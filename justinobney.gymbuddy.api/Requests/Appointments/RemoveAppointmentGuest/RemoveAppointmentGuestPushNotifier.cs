using System.Data.Entity;
using System.Linq;
using justinobney.gymbuddy.api.Data.Appointments;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Interfaces;
using justinobney.gymbuddy.api.Notifications;

namespace justinobney.gymbuddy.api.Requests.Appointments.RemoveAppointmentGuest
{
    public class RemoveAppointmentGuestPushNotifier : IPostRequestHandler<RemoveAppointmentGuestCommand, Appointment>
    {
        private readonly IDbSet<Appointment> _appointments;
        private readonly IDbSet<User> _users;
        private readonly IPushNotifier _pushNotifier;

        public RemoveAppointmentGuestPushNotifier(IDbSet<Appointment> appointments, IDbSet<User> users, IPushNotifier pushNotifier)
        {
            _appointments = appointments;
            _users = users;
            _pushNotifier = pushNotifier;
        }

        public void Notify(RemoveAppointmentGuestCommand request, Appointment response)
        {
            var appt = _appointments
                .First(x => x.Id == request.AppointmentId);

            var apptOwner = _users
                .Include(x => x.Devices)
                .First(x => x.Id == appt.UserId);

            var guest = _users.First(x => x.Id == request.UserId);

            var additionalData = new AdditionalData { Type = NofiticationTypes.RemoveAppointmentGuest };
            var message = new NotificationPayload(additionalData)
            {
                Alert = $"{guest.Name} left your plans",
                Title = "Appointment Guest Left :("
            };
            
            _pushNotifier.Send(message, apptOwner.Devices.AsQueryable());
        }

    }
}