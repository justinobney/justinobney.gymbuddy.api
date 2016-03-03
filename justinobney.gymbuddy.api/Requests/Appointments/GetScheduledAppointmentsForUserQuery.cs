using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using justinobney.gymbuddy.api.Data.Appointments;
using justinobney.gymbuddy.api.Enums;
using justinobney.gymbuddy.api.Requests.Decorators;
using MediatR;

namespace justinobney.gymbuddy.api.Requests.Appointments
{
    public class GetScheduledAppointmentsForUserQuery : IRequest<IQueryable<Appointment>>
    {
        public long UserId { get; set; }
    }

    [DoNotValidate]
    public class GetScheduledAppointmentsForUserQueryHandler :
        IRequestHandler<GetScheduledAppointmentsForUserQuery, IQueryable<Appointment>>
    {
        private readonly IDbSet<Appointment> _appointments;

        public GetScheduledAppointmentsForUserQueryHandler(IDbSet<Appointment> appointments)
        {
            _appointments = appointments;
        }

        public IQueryable<Appointment> Handle(GetScheduledAppointmentsForUserQuery message)
        {
            return _appointments.Where(
                appt =>
                    appt.TimeSlots.Any(ts => ts.Time > DateTime.UtcNow)
                    && (
                        appt.UserId == message.UserId
                        ||
                        appt.GuestList.Any(
                            guest =>
                                guest.UserId == message.UserId
                                && ((appt.Status == AppointmentStatus.Confirmed && guest.Status == AppointmentGuestStatus.Confirmed)
                                    || appt.Status == AppointmentStatus.PendingGuestConfirmation)
                            )
                        )
                );
        }
    }
}