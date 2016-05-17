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

        public DeleteUserCommandHandler(IDbSet<User> users, IDbSet<Appointment> appointments, IDbSet<AppointmentGuest> appointmentGuests)
        {
            _users = users;
            _appointments = appointments;
            _appointmentGuests = appointmentGuests;
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