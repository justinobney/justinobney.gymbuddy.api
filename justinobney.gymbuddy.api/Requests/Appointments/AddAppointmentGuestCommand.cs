using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using justinobney.gymbuddy.api.Data.Appointments;
using justinobney.gymbuddy.api.Enums;
using MediatR;

namespace justinobney.gymbuddy.api.Requests.Appointments
{
    public class AddAppointmentGuestCommand : IAsyncRequest<Appointment>
    {
        public long UserId { get; set; }
        public long AppointmentId { get; set; }
        public long AppointmentTimeSlotId { get; set; }
    }

    public class AddAppointmentGuestCommandHandler : IAsyncRequestHandler<AddAppointmentGuestCommand, Appointment>
    {
        private readonly AppointmentRepository _apptRepo;

        public AddAppointmentGuestCommandHandler(AppointmentRepository apptRepo)
        {
            _apptRepo = apptRepo;
        }

        public async Task<Appointment> Handle(AddAppointmentGuestCommand message)
        {
            var appt = await _apptRepo.Find(x => x.Id == message.AppointmentId)
                .Include(x => x.GuestList)
                .FirstOrDefaultAsync();

            appt.GuestList.Add(new AppointmentGuest
            {
                UserId = message.UserId,
                AppointmentTimeSlotId = message.AppointmentTimeSlotId,
                Status = AppointmentGuestStatus.Pending
            });

            await _apptRepo.UpdateAsync(appt);

            return appt;
        }
    }

    public class AddAppointmentGuestCommandValidator : AbstractValidator<AddAppointmentGuestCommand>
    {
        private readonly AppointmentRepository _apptRepo;

        public AddAppointmentGuestCommandValidator(AppointmentRepository apptRepo)
        {
            _apptRepo = apptRepo;

            CustomAsync(async command =>
            {
                var appt = await _apptRepo.Find(x => x.Id == command.AppointmentId).Include(x => x.GuestList).FirstAsync();
                var isDuplicateGuest =
                    appt.GuestList.Any(
                        guest =>
                            guest.UserId == command.UserId &&
                            guest.AppointmentTimeSlotId == command.AppointmentTimeSlotId);

                return isDuplicateGuest ? new ValidationFailure("UserId", "This user is already registered for this time slot") : null;
            });
        }
    }
}