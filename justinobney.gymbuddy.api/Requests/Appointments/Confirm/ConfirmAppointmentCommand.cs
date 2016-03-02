using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using justinobney.gymbuddy.api.Data.Appointments;
using justinobney.gymbuddy.api.Enums;
using MediatR;

namespace justinobney.gymbuddy.api.Requests.Appointments.Confirm
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
                .Include(x => x.GuestList)
                .Include(x => x.TimeSlots)
                .Include(x => x.User)
                .First(x => x.Id == message.AppointmentId);

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
}