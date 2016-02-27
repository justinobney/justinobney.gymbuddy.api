using System.Data.Entity;
using System.Linq;
using FluentValidation;
using FluentValidation.Results;
using justinobney.gymbuddy.api.Data.Appointments;

namespace justinobney.gymbuddy.api.Requests.Appointments.AddAppointmentGuest
{
    public class AddAppointmentGuestCommandValidator : AbstractValidator<AddAppointmentGuestCommand>
    {

        public AddAppointmentGuestCommandValidator(IDbSet<Appointment> appointments, IDbSet<AppointmentGuest> appointmentGuests)
        {
            Custom(command =>
            {
                var exists = appointments.Any(appt => appt.Id == command.AppointmentId && appt.UserId == command.UserId);

                return exists ? new ValidationFailure("UserId", "You can not be your own guest") : null;
            });

            Custom(command =>
            {
                var exists = appointments.Any(appt =>appt.Id == command.AppointmentId);

                return !exists ? new ValidationFailure("AppointmentId", "This appointment does not exist") : null;
            });

            Custom(command =>
            {
                var isDuplicateGuest = appointmentGuests
                    .Any(guest =>
                        guest.UserId == command.UserId
                        && guest.AppointmentTimeSlotId == command.AppointmentTimeSlotId);

                
                return isDuplicateGuest ? new ValidationFailure("UserId", "This user is already registered for this time slot") : null;
            });
        }
    }
}