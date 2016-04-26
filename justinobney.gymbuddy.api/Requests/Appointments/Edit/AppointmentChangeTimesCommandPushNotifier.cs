using System.Data.Entity;
using System.Linq;
using justinobney.gymbuddy.api.Data.Appointments;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Interfaces;
using justinobney.gymbuddy.api.Notifications;

namespace justinobney.gymbuddy.api.Requests.Appointments.Edit
{
    public class AppointmentChangeTimesCommandPushNotifier : IPostRequestHandler<AppointmentChangeTimesCommand, Appointment>
    {
        private readonly IDbSet<User> _users;
        private readonly IDbSet<AppointmentGuest> _guests;
        private readonly OtherPartiesNotifier _otherPartiesNotifier;

        public AppointmentChangeTimesCommandPushNotifier(IDbSet<User> users, IDbSet<AppointmentGuest> guests, OtherPartiesNotifier otherPartiesNotifier)
        {
            _users = users;
            _guests = guests;
            _otherPartiesNotifier = otherPartiesNotifier;
        }

        public void Notify(AppointmentChangeTimesCommand request, Appointment response)
        {
            var notifier = _users.First(x => x.Id == request.UserId);

            var additionalData = new AdditionalData { Type = NofiticationTypes.AddComment };
            var message = new NotificationPayload(additionalData)
            {
                Title = "GymSquad",
                Alert = $"[Appointment] {notifier.Name} changed the available times. You're request to join has been removed."
            };

            var notifierRequest = new OtherPartiesNotifierRequest
            {
                Guests = _guests.Where(x=>x.AppointmentId == request.AppointmentId),
                AppointmentId = request.AppointmentId,
                UserId = request.UserId,
                AdditionalData = additionalData,
                IncludePending = true
            };
            _otherPartiesNotifier.Send(notifierRequest, message);
        }
    }
}