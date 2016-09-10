using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using justinobney.gymbuddy.api.Data.Appointments;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Enums;
using justinobney.gymbuddy.api.Requests.Decorators;
using MediatR;

namespace justinobney.gymbuddy.api.Requests.Appointments
{
    public class GetAvailableAppointmentsForUserQuery : IRequest<IQueryable<Appointment>>
    {
        public long UserId { get; set; }
        public List<long> GymIds { get; set; }
    }

    [DoNotValidate]
    public class GetAvailableAppointmentsForUserQueryHandler : IRequestHandler<GetAvailableAppointmentsForUserQuery, IQueryable<Appointment>>
    {
        private readonly IDbSet<Appointment> _appointments;
        private readonly IDbSet<Friendship> _friendships;
        private readonly IDbSet<User> _users;
        
        public GetAvailableAppointmentsForUserQueryHandler(
            IDbSet<Appointment> appointments,
            IDbSet<Friendship> friendships,
            IDbSet<User> users
            )
        {
            _appointments = appointments;
            _friendships = friendships;
            _users = users;
        }

        public IQueryable<Appointment> Handle(GetAvailableAppointmentsForUserQuery message)
        {
            var friendUserIds =
                _friendships.Where(x => x.UserId == message.UserId && x.Status == Enums.FriendshipStatus.Active)
                    .Select(x => x.FriendId)
                    .ToList();

            var user = _users.First(x => x.Id == message.UserId);
            var minTime = DateTime.UtcNow.AddMinutes(-30);

            Expression<Func<Appointment, bool>> predicate = appt =>
                (
                friendUserIds.Contains(appt.UserId) ||
                (appt.GymId.HasValue && message.GymIds.Contains(appt.GymId.Value))
                )
                && appt.Status != AppointmentStatus.Confirmed
                && appt.TimeSlots.Any(ts => ts.Time > minTime);

            var possibleAppointments = _appointments
                .Include(x=>x.User)
                .Where(predicate);

            var possibleAppointmentUsers = possibleAppointments
                .Select(x => x.User)
                .Where(UserPredicates.RestrictMemberByGender(user));

            return possibleAppointments
                .Where(x=>possibleAppointmentUsers.Contains(x.User))
                .OrderBy(x=>x.TimeSlots.Select(y=>y.Time).Min());
        }
    }
}