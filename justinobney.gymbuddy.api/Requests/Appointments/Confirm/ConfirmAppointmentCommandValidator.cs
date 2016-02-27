using FluentValidation;

namespace justinobney.gymbuddy.api.Requests.Appointments.Confirm
{
    public class ConfirmAppointmentCommandValidator : AbstractValidator<ConfirmAppointmentCommand>
    {
        public ConfirmAppointmentCommandValidator()
        {
            RuleFor(x => x.AppointmentId)
                .GreaterThan(0);

            RuleFor(x => x.AppointmentGuestIds)
                .NotNull()
                .Must(list => list.Count > 0);
        }
    }
}