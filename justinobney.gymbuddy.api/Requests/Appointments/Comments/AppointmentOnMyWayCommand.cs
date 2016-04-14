using System;
using System.Data.Entity;
using System.Linq;
using justinobney.gymbuddy.api.Data.Appointments;
using justinobney.gymbuddy.api.Requests.Decorators;
using MediatR;

namespace justinobney.gymbuddy.api.Requests.Appointments.Comments
{
    public class AppointmentOnMyWayCommand : IRequest<Appointment>
    {
        public long AppointmentId { get; set; }
        public long UserId { get; set; }
    }

    [DoNotValidate]
    public class AppointmentOnMyWayCommandHandler : IRequestHandler<AppointmentOnMyWayCommand, Appointment>
    {
        private readonly IDbSet<Appointment> _appointments;
        private readonly IDbSet<AppointmentComment> _comments;

        public AppointmentOnMyWayCommandHandler(IDbSet<Appointment> appointments, IDbSet<AppointmentComment> comments)
        {
            _appointments = appointments;
            _comments = comments;
        }

        public Appointment Handle(AppointmentOnMyWayCommand message)
        {
            var appointment = _appointments
                .Include(appt => appt.GuestList)
                .Include(appt => appt.Comments)
                .Include(appt => appt.User.Devices)
                .First(appt => appt.Id == message.AppointmentId);

            _comments.Add(new AppointmentComment
            {
                AppointmentId = appointment.Id,
                UserId = appointment.UserId,
                CreatedAt = DateTime.UtcNow,
                Text = $"{appointment.User.Name} is on the way to the gym"
            });

            return appointment;
        }
    }
}