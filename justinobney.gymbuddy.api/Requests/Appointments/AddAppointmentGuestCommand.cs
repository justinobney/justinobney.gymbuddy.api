using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using justinobney.gymbuddy.api.Data.Appointments;
using justinobney.gymbuddy.api.Enums;
using justinobney.gymbuddy.api.Requests.Decorators;
using MediatR;

namespace justinobney.gymbuddy.api.Requests.Appointments
{
    public class AddAppointmentGuestCommand : IAsyncRequest<Appointment>
    {
        public long UserId { get; set; }
        public long AppointmentId { get; set; }
        public long AppointmentTimeSlotId { get; set; }
    }

    [Commit]
    public class AddAppointmentGuestCommandHandler : IAsyncRequestHandler<AddAppointmentGuestCommand, Appointment>
    {
        private readonly IDbSet<Appointment> _appointments;

        public AddAppointmentGuestCommandHandler(IDbSet<Appointment> appointments)
        {
            _appointments = appointments;
        }

        public async Task<Appointment> Handle(AddAppointmentGuestCommand message)
        {
            var appt = await _appointments
                .Include(x => x.GuestList)
                .FirstOrDefaultAsync(x => x.Id == message.AppointmentId);

            appt.GuestList.Add(new AppointmentGuest
            {
                UserId = message.UserId,
                AppointmentTimeSlotId = message.AppointmentTimeSlotId,
                Status = AppointmentGuestStatus.Pending
            });
            
            return appt;
        }
    }

    public class AddAppointmentGuestCommandValidator : AbstractValidator<AddAppointmentGuestCommand>
    {

        public AddAppointmentGuestCommandValidator(IDbSet<AppointmentGuest> appointmentGuests)
        {
            CustomAsync(async command =>
            {
                var isDuplicateGuest = await appointmentGuests.AnyAsync(guest =>
                            guest.UserId == command.UserId &&
                            guest.AppointmentTimeSlotId == command.AppointmentTimeSlotId);
                
                return isDuplicateGuest ? new ValidationFailure("UserId", "This user is already registered for this time slot") : null;
            });
        }
    }
}