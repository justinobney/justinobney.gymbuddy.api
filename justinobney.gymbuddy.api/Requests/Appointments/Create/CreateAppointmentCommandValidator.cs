using System.Linq;
using FluentValidation;

namespace justinobney.gymbuddy.api.Requests.Appointments.Create
{
    public class CreateAppointmentCommandValidator : AbstractValidator<CreateAppointmentCommand>
    {
        public CreateAppointmentCommandValidator()
        {
            RuleFor(x => x.Title).NotEmpty();
            RuleFor(x => x.GymId).NotEmpty();
            RuleFor(x => x.UserId)
                .NotEmpty();

            RuleFor(x => x.TimeSlots).Must(list => list.Any())
                .WithMessage("At least one time slot is required");
        }
    }
}