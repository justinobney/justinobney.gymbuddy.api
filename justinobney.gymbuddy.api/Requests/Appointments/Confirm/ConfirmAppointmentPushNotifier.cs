using System.Data.Entity;
using System.Linq;
using justinobney.gymbuddy.api.Data.Appointments;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Enums;
using justinobney.gymbuddy.api.Interfaces;
using justinobney.gymbuddy.api.Notifications;

namespace justinobney.gymbuddy.api.Requests.Appointments.Confirm
{
    public class ConfirmAppointmentPushNotifier : IPostRequestHandler<ConfirmAppointmentCommand, Appointment>
    {
        private readonly IDbSet<Appointment> _appointments;
        private readonly IDbSet<User> _users;
        private readonly IPushNotifier _pushNotifier;

        public ConfirmAppointmentPushNotifier(IDbSet<Appointment> appointments, IDbSet<User> users, IPushNotifier pushNotifier)
        {
            _appointments = appointments;
            _users = users;
            _pushNotifier = pushNotifier;
        }

        public void Notify(ConfirmAppointmentCommand request, Appointment response)
        {
            var appt = _appointments
                .Include(x=>x.User)
                .Include(x=>x.GuestList)
                .First(x => x.Id == request.AppointmentId);

            var approvedGuests = appt.GuestList
                .ToList()
                .Where(x => x.Status == AppointmentGuestStatus.Confirmed)
                .Select(x => x.UserId);

            var guests = _users
                .Include(x => x.Devices)
                .Where(x => approvedGuests.Contains(x.Id));

            var additionalData = new AdditionalData { Type = NofiticationTypes.ConfirmAppointment };
            var message = new NotificationPayload(additionalData)
            {
                Alert = $"{appt.User.Name} locked.",
                Title = "Workout Session Locked"
            };

            _pushNotifier.Send(message, guests.SelectMany(x => x.Devices));
        }

    }
}