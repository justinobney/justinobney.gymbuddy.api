using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using justinobney.gymbuddy.api.Data.Appointments;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Requests.Decorators;
using MediatR;

namespace justinobney.gymbuddy.api.Requests.Appointments.Delete
{
    public class DeleteAppointmentCommand : IRequest<Appointment>
    {
        public long Id { get; set; }
        public string NotificaitonAlert { get; set; }
        public string NotificaitonTitle { get; set; }
        public IEnumerable<User> Guests { get; set; }
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
                .Include(x=>x.User)
                .Include(x=>x.GuestList)
                .Include(x=>x.TimeSlots)
                .First(x => x.Id == message.Id);
            
            message.Guests = appt.GuestList.Select(x=>x.User).ToList();
            message.NotificaitonAlert = $"{appt.User.Name} canceled";
            message.NotificaitonTitle = "Workout Session Canceled";

            _appointments.Remove(appt);

            return appt;
        }
    }
}