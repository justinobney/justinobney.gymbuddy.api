using System;
using System.Data.Entity;
using System.Linq;
using justinobney.gymbuddy.api.Data.Appointments;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Requests.Decorators;
using MediatR;

namespace justinobney.gymbuddy.api.Requests.Users
{
    public class DeleteUserCommand : IRequest<User>
    {
        public long Id { get; set; }
    }

    [DoNotValidate]
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, User>
    {
        private readonly IDbSet<User> _users;
        private readonly IDbSet<Appointment> _appointments;
        private readonly IDbSet<AppointmentGuest> _appointmentGuests;
        private readonly IDbSet<AppointmentComment> _appointmentComments;

        public DeleteUserCommandHandler(
            IDbSet<User> users,
            IDbSet<Appointment> appointments,
            IDbSet<AppointmentGuest> appointmentGuests,
            IDbSet<AppointmentComment> appointmentComments 
        )
        {
            _users = users;
            _appointments = appointments;
            _appointmentGuests = appointmentGuests;
            _appointmentComments = appointmentComments;
        }

        public User Handle(DeleteUserCommand message)
        {
            var user = _users
                .Include(x=>x.Appointments)
                .FirstOrDefault(x => x.Id == message.Id);

            if (user == null)
            {
                throw new ArgumentException("User not found");
            }

            var comments = _appointmentComments.Where(x => x.UserId == user.Id).ToList();
            foreach (var comment in comments)
            {
                _appointmentComments.Remove(comment);
            }

            var guestEntry = _appointmentGuests.Where(x => x.UserId == user.Id).ToList();
            foreach (var appointmentGuest in guestEntry)
            {
                _appointmentGuests.Remove(appointmentGuest);
            }

            foreach (var appt in user.Appointments.ToList())
            {
                _appointments.Remove(appt);
            }

            _users.Remove(user);
            return user;
        }
    }
}