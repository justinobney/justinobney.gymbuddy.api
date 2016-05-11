using System.Data.Entity;
using System.Linq;
using justinobney.gymbuddy.api.Data.Appointments;
using justinobney.gymbuddy.api.Enums;
using justinobney.gymbuddy.api.Requests.Decorators;
using MediatR;

namespace justinobney.gymbuddy.api.Requests.Appointments.AddAppointmentGuest
{
    public class AddAppointmentGuestCommand : IRequest<AppointmentGuest>
    {
        public long UserId { get; set; }
        public long AppointmentId { get; set; }
        public long AppointmentTimeSlotId { get; set; }
    }

    public class AddAppointmentGuestCommandHandler : IRequestHandler<AddAppointmentGuestCommand, AppointmentGuest>
    {
        private readonly IDbSet<Appointment> _appointments;
        private readonly IDbSet<AppointmentGuest> _guests;

        public AddAppointmentGuestCommandHandler(
            IDbSet<Appointment> appointments,
            IDbSet<AppointmentGuest> guests
            )
        {
            _appointments = appointments;
            _guests = guests;
        }

        public AppointmentGuest Handle(AddAppointmentGuestCommand message)
        {
            var appt = _appointments
                .Include(x => x.GuestList)
                .Include(x => x.TimeSlots)
                .FirstOrDefault(x => x.Id == message.AppointmentId);

            var guest = new AppointmentGuest
            {
                UserId = message.UserId,
                AppointmentTimeSlotId = message.AppointmentTimeSlotId,
                AppointmentId = message.AppointmentId,
                Status = AppointmentGuestStatus.Pending
            };

            _guests.Add(guest);

            appt.Status = AppointmentStatus.PendingGuestConfirmation;

            return guest;
        }
    }
}