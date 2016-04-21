using System;
using System.Data.Entity;
using System.Linq;
using justinobney.gymbuddy.api.Data.Appointments;
using justinobney.gymbuddy.api.Data.Users;
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
        private readonly IDbSet<User> _users;

        public AppointmentOnMyWayCommandHandler(
            IDbSet<Appointment> appointments,
            IDbSet<AppointmentComment> comments,
            IDbSet<User> users 
            )
        {
            _appointments = appointments;
            _comments = comments;
            _users = users;
        }

        public Appointment Handle(AppointmentOnMyWayCommand message)
        {
            var notifier = _users.First(x => x.Id == message.UserId);

            var appointment = _appointments
                .Include(appt => appt.GuestList)
                .Include(appt => appt.Comments)
                .Include(appt => appt.User.Devices)
                .First(appt => appt.Id == message.AppointmentId);

            _comments.Add(new AppointmentComment
            {
                AppointmentId = appointment.Id,
                UserId = notifier.Id,
                CreatedAt = DateTime.UtcNow,
                Text = $"{notifier.Name} is on the way to the gym"
            });

            return appointment;
        }
    }
}