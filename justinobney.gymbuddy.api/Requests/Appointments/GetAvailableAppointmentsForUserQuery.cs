using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using justinobney.gymbuddy.api.Data.Appointments;
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

        public GetAvailableAppointmentsForUserQueryHandler(IDbSet<Appointment> appointments)
        {
            _appointments = appointments;
        }

        public IQueryable<Appointment> Handle(GetAvailableAppointmentsForUserQuery message)
        {
            return _appointments.Where(appt =>
                message.GymIds.Contains(appt.GymId.Value)
                && appt.Status != AppointmentStatus.Confirmed
                && appt.TimeSlots.Any(ts => ts.Time > DateTime.UtcNow));
        }
    }
}