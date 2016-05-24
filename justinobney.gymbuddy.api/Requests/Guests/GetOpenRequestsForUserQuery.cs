using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using justinobney.gymbuddy.api.Data.Appointments;
using justinobney.gymbuddy.api.Enums;
using justinobney.gymbuddy.api.Requests.Decorators;
using MediatR;

namespace justinobney.gymbuddy.api.Requests.Guests
{
    public class GetOpenRequestsForUserQuery : IRequest<IQueryable<AppointmentGuest>>
    {
        public long UserId { get; set; }
    }

    [DoNotValidate]
    [DoNotCommit]
    public class GetOpenRequestsForUserQueryHandler : IRequestHandler<GetOpenRequestsForUserQuery, IQueryable<AppointmentGuest>>
    {
        private readonly IDbSet<Appointment> _appointments;
        private readonly IDbSet<AppointmentGuest> _guests;

        public GetOpenRequestsForUserQueryHandler(IDbSet<Appointment> appointments, IDbSet<AppointmentGuest> guests)
        {
            _appointments = appointments;
            _guests = guests;
        }

        public IQueryable<AppointmentGuest> Handle(GetOpenRequestsForUserQuery message)
        {
            var minTime = DateTime.UtcNow.AddMinutes(-30);
            Expression<Func<Appointment, bool>> predicate = appt =>
                appt.Status != AppointmentStatus.Confirmed
                && appt.UserId == message.UserId
                && appt.TimeSlots.Any(ts => ts.Time > minTime);

            var possibleAppts = _appointments.Where(predicate).Select(x => x.Id).ToList();

            return _guests.Where(
                x =>
                    possibleAppts.Contains(x.AppointmentId)
                    && x.Status == AppointmentGuestStatus.Pending
                );
        }
    }
}