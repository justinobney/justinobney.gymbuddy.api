using System.Data.Entity;
using System.Linq;
using justinobney.gymbuddy.api.Data.Appointments;
using justinobney.gymbuddy.api.Requests.Decorators;
using MediatR;

namespace justinobney.gymbuddy.api.Requests.Appointments.RemoveAppointmentGuest
{
    public class RemoveAppointmentGuestCommand : IRequest<Appointment>
    {
        public long GuestAppointmentId { get; set; }
        public long AppointmentId { get; set; }
        public long UserId { get; set; }
    }

    [DoNotValidate]
    public class RemoveAppointmentGuestCommandHandler : IRequestHandler<RemoveAppointmentGuestCommand, Appointment>
    {
        private readonly IDbSet<Appointment> _appointments;
        private readonly IDbSet<AppointmentGuest> _appointmentGuests;

        public RemoveAppointmentGuestCommandHandler(IDbSet<Appointment> appointments, IDbSet<AppointmentGuest> appointmentGuests)
        {
            _appointments = appointments;
            _appointmentGuests = appointmentGuests;
        }

        public Appointment Handle(RemoveAppointmentGuestCommand message)
        {
            var guestAppt = _appointmentGuests
                .First(x=> x.Id == message.GuestAppointmentId);
            
            var appt = _appointments
                .Include(x => x.GuestList)
                .First(appointment => appointment.Id == guestAppt.AppointmentId);

            _appointmentGuests.Remove(guestAppt);

            // For notifications
            message.AppointmentId = appt.Id;
            message.UserId = guestAppt.UserId;

            return appt;
        }
    }
}