using System.Data.Entity;
using System.Linq;
using justinobney.gymbuddy.api.Data.Appointments;
using justinobney.gymbuddy.api.Enums;
using MediatR;

namespace justinobney.gymbuddy.api.Requests.Appointments.AddAppointmentGuest
{
    public class AddAppointmentGuestCommand : IRequest<Appointment>
    {
        public long UserId { get; set; }
        public long AppointmentId { get; set; }
        public long AppointmentTimeSlotId { get; set; }
    }

    public class AddAppointmentGuestCommandHandler : IRequestHandler<AddAppointmentGuestCommand, Appointment>
    {
        private readonly IDbSet<Appointment> _appointments;

        public AddAppointmentGuestCommandHandler(IDbSet<Appointment> appointments)
        {
            _appointments = appointments;
        }

        public Appointment Handle(AddAppointmentGuestCommand message)
        {
            var appt = _appointments
                .Include(x => x.GuestList)
                .FirstOrDefault(x => x.Id == message.AppointmentId);

            appt.GuestList.Add(new AppointmentGuest
            {
                UserId = message.UserId,
                AppointmentTimeSlotId = message.AppointmentTimeSlotId,
                Status = AppointmentGuestStatus.Pending
            });

            appt.Status = AppointmentStatus.PendingGuestConfirmation;

            return appt;
        }
    }
}