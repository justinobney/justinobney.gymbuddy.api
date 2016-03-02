using System.Data.Entity;
using System.Linq;
using justinobney.gymbuddy.api.Data.Appointments;
using justinobney.gymbuddy.api.Enums;
using MediatR;

namespace justinobney.gymbuddy.api.Requests.Appointments.Confirm
{
    public class ConfirmAppointmentGuestCommand : IRequest<AppointmentGuest>
    {
        public long AppointmentId { get; set; }
        public long AppointmentGuestId { get; set; }
    }

    public class ConfirmAppointmentGuestCommandHandler : IRequestHandler<ConfirmAppointmentGuestCommand, AppointmentGuest>
    {
        private readonly IDbSet<AppointmentGuest> _guests;

        public ConfirmAppointmentGuestCommandHandler(IDbSet<AppointmentGuest> guests)
        {
            _guests = guests;
        }

        public AppointmentGuest Handle(ConfirmAppointmentGuestCommand message)
        {
            var guest = _guests
                .Include(x => x.TimeSlot)
                .Include(x => x.User)
                .First(x => x.Id == message.AppointmentGuestId);

            guest.Status = AppointmentGuestStatus.Confirmed;

            return guest;
        }
    }
}