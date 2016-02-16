using System.Data.Entity;
using System.Threading.Tasks;
using justinobney.gymbuddy.api.Data.Appointments;
using justinobney.gymbuddy.api.Requests.Decorators;
using MediatR;

namespace justinobney.gymbuddy.api.Requests.Appointments
{
    public class RemoveAppointmentGuestCommand : IAsyncRequest<Appointment>
    {
        public long GuestAppointmentId { get; set; }
    }

    [DoNotValidate]
    [Commit]
    public class RemoveAppointmentGuestCommandHandler : IAsyncRequestHandler<RemoveAppointmentGuestCommand, Appointment>
    {
        private readonly IDbSet<Appointment> _appointments;
        private readonly IDbSet<AppointmentGuest> _appointmentGuests;

        public RemoveAppointmentGuestCommandHandler(IDbSet<Appointment> appointments, IDbSet<AppointmentGuest> appointmentGuests)
        {
            _appointments = appointments;
            _appointmentGuests = appointmentGuests;
        }

        public async Task<Appointment> Handle(RemoveAppointmentGuestCommand message)
        {
            var guestAppt = await _appointmentGuests.FirstOrDefaultAsync(x=> x.Id == message.GuestAppointmentId);
            _appointmentGuests.Remove(guestAppt);


            var appt = await _appointments
                .Include(x => x.GuestList)
                .FirstOrDefaultAsync(appointment => appointment.Id == guestAppt.AppointmentId);

            return appt;
        }
    }
}