using System;
using System.Data.Entity;
using System.Linq;
using justinobney.gymbuddy.api.Data;
using justinobney.gymbuddy.api.Data.Appointments;
using justinobney.gymbuddy.api.Data.Notifications;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Helpers;
using justinobney.gymbuddy.api.Interfaces;
using justinobney.gymbuddy.api.Notifications;
using justinobney.gymbuddy.api.Responses;
using Newtonsoft.Json;

namespace justinobney.gymbuddy.api.Requests.Appointments.AddAppointmentGuest
{
    public class AddAppointmentGuestPushNotifier : IPostRequestHandler<AddAppointmentGuestCommand, AppointmentGuest>
    {
        private readonly IDbSet<Appointment> _appointments;
        private readonly IDbSet<Notification> _notifications;
        private readonly IDbSet<User> _users;
        private readonly AppContext _context;
        private readonly IPushNotifier _pushNotifier;

        public AddAppointmentGuestPushNotifier(
            IDbSet<Appointment> appointments,
            IDbSet<Notification> notifications,
            IDbSet<User> users,
            AppContext context,
            IPushNotifier pushNotifier)
        {
            _appointments = appointments;
            _notifications = notifications;
            _users = users;
            _context = context;
            _pushNotifier = pushNotifier;
        }

        public void Notify(AddAppointmentGuestCommand request, AppointmentGuest response)
        {
            var appt = _appointments.First(x => x.Id == request.AppointmentId);
            var apptOwner = _users
                .Include(x=>x.Devices)
                .First(x => x.Id == appt.UserId);

            var guest = _users.First(x => x.Id == request.UserId);

            _notifications.Add(new Notification
            {
                UserId = apptOwner.Id,
                Type = NofiticationTypes.AddAppointmentGuest,
                Message = $"{guest.Name} wants to work in",
                Title = "Appointment Guest Request",
                CreatedAt = DateTime.UtcNow,
                JsonPayload = JsonConvert.SerializeObject(
                    MappingConfig.Instance.Map<AppointmentGuestListing>(response), StaticConfig.JsonSerializerSettings
                    )
            });

            _context.SaveChanges();

            var additionalData = new AdditionalData
            {
                Type = NofiticationTypes.AddAppointmentGuest,
                AppointmentId = response.AppointmentId
            };
            var message = new NotificationPayload(additionalData)
            {
                Alert = $"{guest.Name} wants to work in",
                Title = "Appointment Guest Request"
            };
            
            _pushNotifier.Send(message, apptOwner.Devices.AsQueryable());
        }

    }
}