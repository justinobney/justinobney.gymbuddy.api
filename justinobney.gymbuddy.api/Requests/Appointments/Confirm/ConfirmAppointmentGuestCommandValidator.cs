using FluentValidation;

namespace justinobney.gymbuddy.api.Requests.Appointments.Confirm
{
    public class ConfirmAppointmentGuestCommandValidator : AbstractValidator<ConfirmAppointmentGuestCommand>
    {
        public ConfirmAppointmentGuestCommandValidator()
        {
            RuleFor(x => x.AppointmentGuestId).NotEmpty();
        }
    }
}