using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using FluentValidation;
using justinobney.gymbuddy.api.Data.Appointments;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Enums;
using justinobney.gymbuddy.api.Interfaces;
using justinobney.gymbuddy.api.Notifications;
using MediatR;
using RestSharp;

namespace justinobney.gymbuddy.api.Requests.Appointments
{
    public class ConfirmAppointmentCommand : IRequest<Appointment>
    {
        public long AppointmentId { get; set; }
        public List<long> AppointmentGuestIds { get; set; } = new List<long>();
    }

    public class ConfirmAppointmentCommandHandler : IRequestHandler<ConfirmAppointmentCommand, Appointment>
    {
        private readonly IDbSet<Appointment> _appointments;

        public ConfirmAppointmentCommandHandler(IDbSet<Appointment> appointments)
        {
            _appointments = appointments;
        }
        
        public Appointment Handle(ConfirmAppointmentCommand message)
        {
            var appt = _appointments
                .Include(x=>x.GuestList)
                .Include(x=>x.TimeSlots)
                .First(x=>x.Id == message.AppointmentId);

            var approvedGuests = appt.GuestList
                .Where(guest => message.AppointmentGuestIds.Any(id => id == guest.Id))
                .ToList();

            var timeslot = appt.TimeSlots.First(x => x.Id == approvedGuests.First().AppointmentTimeSlotId);

            approvedGuests.ForEach(guest => guest.Status = AppointmentGuestStatus.Confirmed);

            appt.ConfirmedTime = timeslot.Time;
            appt.Status = AppointmentStatus.Confirmed;

            return appt;
        }
    }

    public class ConfirmAppointmentCommandValidator : AbstractValidator<ConfirmAppointmentCommand>
    {
        public ConfirmAppointmentCommandValidator()
        {
            RuleFor(x => x.AppointmentId)
                .GreaterThan(0);

            RuleFor(x => x.AppointmentGuestIds)
                .NotNull()
                .Must(list => list.Count > 0);
        }
    }

    public class ConfirmAppointmentPushNotifier : IPostRequestHandler<ConfirmAppointmentCommand, Appointment>
    {
        private readonly IDbSet<Appointment> _appointments;
        private readonly IDbSet<User> _users;
        private readonly IRestClient _client;

        public ConfirmAppointmentPushNotifier(IDbSet<Appointment> appointments, IDbSet<User> users, IRestClient client)
        {
            _appointments = appointments;
            _users = users;
            _client = client;
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

            var message = new NotificationPayload<object>(null)
            {
                Alert = $"{appt.User.Name} confirmed.",
                Title = "Workout Session Confirmed"
            };

            var iosNotification = new IonicPushNotification(message)
            {
                Production = true,
                Tokens = guests.SelectMany(x => x.Devices
                    .Where(y => y.Platform == "iOS" && !string.IsNullOrEmpty(y.PushToken))
                    .Select(y => y.PushToken))
                    .ToList()
            };

            var androidNotification = new IonicPushNotification(message)
            {
                Tokens = guests.SelectMany(x => x.Devices
                    .Where(y => y.Platform == "Android" && !string.IsNullOrEmpty(y.PushToken))
                    .Select(y => y.PushToken))
                    .ToList()
            };

            iosNotification.Send(_client);
            androidNotification.Send(_client);
        }

    }
}