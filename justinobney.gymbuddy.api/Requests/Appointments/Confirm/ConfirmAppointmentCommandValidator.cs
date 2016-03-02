using System.Data.Entity;
using System.Linq;
using FluentValidation;
using FluentValidation.Results;
using justinobney.gymbuddy.api.Data.Appointments;
using justinobney.gymbuddy.api.Enums;

namespace justinobney.gymbuddy.api.Requests.Appointments.Confirm
{
    public class ConfirmAppointmentCommandValidator : AbstractValidator<ConfirmAppointmentCommand>
    {
        public ConfirmAppointmentCommandValidator(IDbSet<Appointment> appointments)
        {
            RuleFor(x => x.AppointmentId)
                .GreaterThan(0);

            Custom(command =>
            {
                var appt = appointments
                    .Include(x => x.GuestList)
                    .Include(x => x.TimeSlots)
                    .FirstOrDefault(x => x.Id == command.AppointmentId);

                return appt == null ||  appt.GuestList.Any(x => x.Status == AppointmentGuestStatus.Confirmed)
                    ? null
                    : new ValidationFailure("AppointmentId", "There are no confirmed guests");
            });
            
        }
    }
}