using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using FluentValidation;
using justinobney.gymbuddy.api.Data.Appointments;
using justinobney.gymbuddy.api.Enums;
using MediatR;
using WebGrease.Css.Extensions;

namespace justinobney.gymbuddy.api.Requests.Appointments
{
    public class ConfirmAppointmentCommand : IRequest<Appointment>
    {
        public long AppointmentId { get; set; }
        public List<long> AppointmentGuestIds { get; set; } = new List<long>();
    }

    public class ConfirmAppointmentCommandHandler : IRequestHandler<ConfirmAppointmentCommand, Appointment>
    {
        private readonly IDbSet<Appointment> _appontments;

        public ConfirmAppointmentCommandHandler(IDbSet<Appointment> appontments)
        {
            _appontments = appontments;
        }

        public Appointment Handle(ConfirmAppointmentCommand message)
        {
            var appt = _appontments
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
}