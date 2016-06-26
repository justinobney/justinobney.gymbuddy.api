using System.Data.Entity;
using System.Linq;
using justinobney.gymbuddy.api.Data.Appointments;
using justinobney.gymbuddy.api.Interfaces;
using justinobney.gymbuddy.api.Notifications;

namespace justinobney.gymbuddy.api.Requests.Appointments.Confirm
{
    public class ConfirmAppointmentGuestPushNotifier : IPostRequestHandler<ConfirmAppointmentGuestCommand, AppointmentGuest>
    {
        private readonly IDbSet<Appointment> _appointments;
        private readonly IDbSet<AppointmentGuest> _guests;
        private readonly IPushNotifier _pushNotifier;

        public ConfirmAppointmentGuestPushNotifier(IDbSet<Appointment> appointments, IDbSet<AppointmentGuest> guests, IPushNotifier pushNotifier)
        {
            _appointments = appointments;
            _guests = guests;
            _pushNotifier = pushNotifier;
        }

        public void Notify(ConfirmAppointmentGuestCommand request, AppointmentGuest response)
        {
            var appt = _appointments
                .Include(x => x.User)
                .Include(x => x.GuestList)
                .First(x => x.Id == request.AppointmentId);

            var guest = _guests
                .Include(x=>x.User.Devices)
                .First(x => x.Id == request.AppointmentGuestId);
            
            var additionalData = new AdditionalData
            {
                Type = NofiticationTypes.ConfirmAppointmentGuest,
                AppointmentId = response.AppointmentId
            };
            var message = new NotificationPayload(additionalData)
            {
                Message = $"{appt.User.Name} confirmed.",
                Title = "Workout Session Confirmed"
            };

            _pushNotifier.Send(message, guest.User.Devices.AsQueryable());
        }
    }
}