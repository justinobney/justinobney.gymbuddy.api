using System.Linq;
using FluentValidation;

namespace justinobney.gymbuddy.api.Requests.Appointments.Edit
{
    public class AppointmentChangeTimesCommandValidator : AbstractValidator<AppointmentChangeTimesCommand>
    {
        public AppointmentChangeTimesCommandValidator()
        {
            RuleFor(x => x.AppointmentId).NotEmpty();
            RuleFor(x => x.UserId).NotEmpty();

            RuleFor(x => x.TimeSlots).Must(list => list.Any())
                .WithMessage("At least one time slot is required");
        }
    }
}