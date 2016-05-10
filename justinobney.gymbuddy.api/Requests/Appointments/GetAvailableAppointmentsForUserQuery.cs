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
        private readonly IDbSet<User> _users;
        
        public GetAvailableAppointmentsForUserQueryHandler(IDbSet<Appointment> appointments, IDbSet<User> users)
        {
            _appointments = appointments;
            _users = users;
        }

        public IQueryable<Appointment> Handle(GetAvailableAppointmentsForUserQuery message)
        {
            var user = _users.First(x => x.Id == message.UserId);
            var minTime = DateTime.UtcNow.AddMinutes(-30);

            Expression<Func<Appointment, bool>> predicate = appt =>
                message.GymIds.Contains(appt.GymId.Value)
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