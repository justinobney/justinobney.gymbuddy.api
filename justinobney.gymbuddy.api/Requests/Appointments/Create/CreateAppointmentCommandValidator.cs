using System.Linq;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Ajax.Utilities;

namespace justinobney.gymbuddy.api.Requests.Appointments.Create
{
    public class CreateAppointmentCommandValidator : AbstractValidator<CreateAppointmentCommand>
    {
        public CreateAppointmentCommandValidator()
        {
            RuleFor(x => x.Title).NotEmpty();
            RuleFor(x => x.UserId)
                .NotEmpty();

            RuleFor(x => x.TimeSlots).Must(list => list.Any())
                .WithMessage("At least one time slot is required");

            Custom(
                command =>
                    !command.Location.IsNullOrWhiteSpace() || command.GymId != 0
                        ? null
                        : new ValidationFailure("Location", "A gym or location is required")
                );
        }
    }
}