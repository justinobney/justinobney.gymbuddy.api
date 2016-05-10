using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using justinobney.gymbuddy.api.Data.Appointments;
using justinobney.gymbuddy.api.Data.Devices;
using justinobney.gymbuddy.api.Enums;
using justinobney.gymbuddy.api.Interfaces;

namespace justinobney.gymbuddy.api.Notifications
{
    public class OtherPartiesNotifierRequest
    {
        public long AppointmentId { get; set; }
        public long UserId { get; set; }
        public AdditionalData AdditionalData { get; set; }
        public bool IncludePending { get; set; }
        public IEnumerable<Device> Devices { get; set; } = new List<Device>();
    }

    public class OtherPartiesNotifier
    {
        private readonly IPushNotifier _pushNotifier;
        private readonly IDbSet<Appointment> _appointments;
        private readonly IDbSet<AppointmentGuest> _guests;
        private readonly IDbSet<AppointmentComment> _comments;

        public OtherPartiesNotifier(
            IPushNotifier pushNotifier,
            IDbSet<Appointment> appointments,
            IDbSet<AppointmentGuest> guests,
            IDbSet<AppointmentComment> comments
            )
        {
            _pushNotifier = pushNotifier;
            _appointments = appointments;
            _guests = guests;
            _comments = comments;
        }

        public void Send(OtherPartiesNotifierRequest notifierRequest, NotificationPayload message)
        {
            IEnumerable<Device> guestDevices = notifierRequest.Devices ?? new List<Device>();

            var appt = _appointments
                .Include(x => x.User.Devices)
                .First(x => x.Id == notifierRequest.AppointmentId);

            var commentorDevices = _comments
                .Where(x => x.AppointmentId == notifierRequest.AppointmentId)
                .SelectMany(x => x.User.Devices);

            if (!guestDevices.Any())
            {
                var guests = _guests
                    .Include(x => x.User.Devices)
                    .Where(x => x.AppointmentId == notifierRequest.AppointmentId);

                if (!notifierRequest.IncludePending)
                {
                    guests = guests.Where(x => x.Status == AppointmentGuestStatus.Confirmed);
                }

                guestDevices = guests.SelectMany(x => x.User.Devices);
            }
            

            var devices = appt
                .User.Devices
                .Concat(guestDevices)
                .Concat(commentorDevices)
                .Where(x => x.UserId != notifierRequest.UserId);

            _pushNotifier.Send(message, devices);
        }
    }
}