using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using justinobney.gymbuddy.api.Data;
using justinobney.gymbuddy.api.Data.Appointments;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Requests.Decorators;
using MediatR;

namespace justinobney.gymbuddy.api.Requests.Users
{
    public class DeleteUserCommand : IAsyncRequest<User>
    {
        public long Id { get; set; }
    }

    [DoNotValidate]
    public class DeleteUserCommandHandler : IAsyncRequestHandler<DeleteUserCommand, User>
    {
        private readonly AppContext _context;
        private readonly IDbSet<User> _users;
        private readonly IDbSet<Appointment> _appointments;
        private readonly IDbSet<AppointmentGuest> _appointmentGuests;

        public DeleteUserCommandHandler(AppContext context, IDbSet<User> users, IDbSet<Appointment> appointments, IDbSet<AppointmentGuest> appointmentGuests)
        {
            _context = context;
            _users = users;
            _appointments = appointments;
            _appointmentGuests = appointmentGuests;
        }

        public async Task<User> Handle(DeleteUserCommand message)
        {
            var user = _users
                .Include(x=>x.Appointments.Select(y=>y.GuestList))
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
            await _context.SaveChangesAsync();
            return user;
        }
    }
}