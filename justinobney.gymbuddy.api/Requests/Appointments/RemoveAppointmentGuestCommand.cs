using System.Data.Entity;
using System.Linq;
using justinobney.gymbuddy.api.Data.Appointments;
using justinobney.gymbuddy.api.Requests.Decorators;
using MediatR;

namespace justinobney.gymbuddy.api.Requests.Appointments
{
    public class RemoveAppointmentGuestCommand : IRequest<Appointment>
    {
        public long GuestAppointmentId { get; set; }
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
            var guestAppt = _appointmentGuests.FirstOrDefault(x=> x.Id == message.GuestAppointmentId);
            _appointmentGuests.Remove(guestAppt);


            var appt = _appointments
                .Include(x => x.GuestList)
                .FirstOrDefault(appointment => appointment.Id == guestAppt.AppointmentId);

            return appt;
        }
    }
}