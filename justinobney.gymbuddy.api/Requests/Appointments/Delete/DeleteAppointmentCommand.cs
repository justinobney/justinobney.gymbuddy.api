using System.Data.Entity;
using System.Linq;
using justinobney.gymbuddy.api.Data.Appointments;
using justinobney.gymbuddy.api.Requests.Decorators;
using MediatR;

namespace justinobney.gymbuddy.api.Requests.Appointments.Delete
{
    public class DeleteAppointmentCommand : IRequest<Appointment>
    {
        public long Id { get; set; }
    }

    [DoNotValidate]
    public class DeleteAppointmentCommandHandler : IRequestHandler<DeleteAppointmentCommand, Appointment>
    {
        private readonly IDbSet<Appointment> _appointments;

        public DeleteAppointmentCommandHandler(IDbSet<Appointment> appointments)
        {
            _appointments = appointments;
        }

        public Appointment Handle(DeleteAppointmentCommand message)
        {
            var appt = _appointments
                .Include(x=>x.GuestList)
                .Include(x=>x.TimeSlots)
                .First(x => x.Id == message.Id);

            _appointments.Remove(appt);

            return appt;
        }
    }
}