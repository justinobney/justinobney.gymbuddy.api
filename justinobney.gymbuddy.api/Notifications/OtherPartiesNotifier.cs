using System.Data.Entity;
using System.Linq;
using justinobney.gymbuddy.api.Data.Appointments;
using justinobney.gymbuddy.api.Enums;

namespace justinobney.gymbuddy.api.Notifications
{
    public class OtherPartiesNotifierRequest
    {
        public long AppointmentId { get; set; }
        public long UserId { get; set; }
        public AdditionalData AdditionalData { get; set; }
    }

    public class OtherPartiesNotifier
    {
        private readonly PushNotifier _pushNotifier;
        private readonly IDbSet<Appointment> _appointments;
        private readonly IDbSet<AppointmentGuest> _guests;

        public OtherPartiesNotifier(
            PushNotifier pushNotifier,
            IDbSet<Appointment> appointments,
            IDbSet<AppointmentGuest> guests
            )
        {
            _pushNotifier = pushNotifier;
            _appointments = appointments;
            _guests = guests;
        }

        public void Send(OtherPartiesNotifierRequest notifierRequest, NotificationPayload message)
        {
            var guestDevices = _guests
                .Include(x => x.User.Devices)
                .Where(x => x.AppointmentId == notifierRequest.AppointmentId)
                .Where(x => x.Status == AppointmentGuestStatus.Confirmed)
                .SelectMany(x => x.User.Devices)
                .AsQueryable();

            var devices = _appointments
                .Include(x => x.User.Devices)
                .First(x => x.Id == notifierRequest.AppointmentId)
                .User.Devices
                .Concat(guestDevices)
                .Where(x => x.UserId != notifierRequest.UserId)
                .AsQueryable();

            _pushNotifier.Send(message, devices);
        }
    }
}