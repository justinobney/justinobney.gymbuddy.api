using System.Data.Entity;
using System.Linq;
using FluentValidation;
using FluentValidation.Results;
using justinobney.gymbuddy.api.Data.Appointments;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Enums;
using justinobney.gymbuddy.api.Interfaces;
using justinobney.gymbuddy.api.Notifications;
using MediatR;
using RestSharp;

namespace justinobney.gymbuddy.api.Requests.Appointments
{
    public class AddAppointmentGuestCommand : IRequest<Appointment>
    {
        public long UserId { get; set; }
        public long AppointmentId { get; set; }
        public long AppointmentTimeSlotId { get; set; }
    }

    public class AddAppointmentGuestCommandHandler : IRequestHandler<AddAppointmentGuestCommand, Appointment>
    {
        private readonly IDbSet<Appointment> _appointments;

        public AddAppointmentGuestCommandHandler(IDbSet<Appointment> appointments)
        {
            _appointments = appointments;
        }

        public Appointment Handle(AddAppointmentGuestCommand message)
        {
            var appt = _appointments
                .Include(x => x.GuestList)
                .FirstOrDefault(x => x.Id == message.AppointmentId);

            appt.GuestList.Add(new AppointmentGuest
            {
                UserId = message.UserId,
                AppointmentTimeSlotId = message.AppointmentTimeSlotId,
                Status = AppointmentGuestStatus.Pending
            });

            appt.Status = AppointmentStatus.PendingGuestConfirmation;

            return appt;
        }
    }

    public class AddAppointmentGuestCommandValidator : AbstractValidator<AddAppointmentGuestCommand>
    {

        public AddAppointmentGuestCommandValidator(IDbSet<Appointment> appointments, IDbSet<AppointmentGuest> appointmentGuests)
        {
            Custom(command =>
            {
                var exists = appointments.Any(appt => appt.Id == command.AppointmentId && appt.UserId == command.UserId);

                return exists ? new ValidationFailure("UserId", "You can not be your own guest") : null;
            });

            Custom(command =>
            {
                var exists = appointments.Any(appt =>appt.Id == command.AppointmentId);

                return !exists ? new ValidationFailure("AppointmentId", "This appointment does not exist") : null;
            });

            Custom(command =>
            {
                var isDuplicateGuest = appointmentGuests
                    .Any(guest =>
                        guest.UserId == command.UserId
                        && guest.AppointmentTimeSlotId == command.AppointmentTimeSlotId);

                
                return isDuplicateGuest ? new ValidationFailure("UserId", "This user is already registered for this time slot") : null;
            });
        }
    }

    public class AddAppointmentGuestPushNotifier : IPostRequestHandler<AddAppointmentGuestCommand, Appointment>
    {
        private readonly IDbSet<Appointment> _appointments;
        private readonly IDbSet<User> _users;
        private readonly IRestClient _client;

        public AddAppointmentGuestPushNotifier(IDbSet<Appointment> appointments, IDbSet<User> users, IRestClient client)
        {
            _appointments = appointments;
            _users = users;
            _client = client;
        }

        public void Notify(AddAppointmentGuestCommand request, Appointment response)
        {
            var appt = _appointments.First(x => x.Id == request.AppointmentId);
            var apptOwner = _users.First(x => x.Id == appt.UserId);
            var guest = _users.First(x => x.Id == request.UserId);

            var message = new NotificationPayload<object>(null)
            {
                Alert = $"{guest.Name} wants to work in",
                Title = "Appointment Guest Request"
            };

            var iosNotification = new IonicPushNotification(message)
            {
                Production = true,
                Tokens = apptOwner.Devices
                    .Where(y => y.Platform == "iOS" && !string.IsNullOrEmpty(y.PushToken))
                    .Select(y => y.PushToken)
                    .ToList()
            };

            var androidNotification = new IonicPushNotification(message)
            {
                Tokens = apptOwner.Devices
                    .Where(y => y.Platform == "Android" && !string.IsNullOrEmpty(y.PushToken))
                    .Select(y => y.PushToken)
                    .ToList()
            };

            iosNotification.Send(_client);
            androidNotification.Send(_client);
        }

    }
}