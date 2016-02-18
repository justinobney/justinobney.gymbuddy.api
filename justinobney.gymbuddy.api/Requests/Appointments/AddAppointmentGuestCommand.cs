using System.Data.Entity;
using System.Linq;
using FluentValidation;
using FluentValidation.Results;
using justinobney.gymbuddy.api.Data.Appointments;
using justinobney.gymbuddy.api.Enums;
using justinobney.gymbuddy.api.Requests.Decorators;
using MediatR;

namespace justinobney.gymbuddy.api.Requests.Appointments
{
    public class AddAppointmentGuestCommand : IRequest<Appointment>
    {
        public long UserId { get; set; }
        public long AppointmentId { get; set; }
        public long AppointmentTimeSlotId { get; set; }
    }

    public class AddAppointmentGuestCommandHandler : IRequestHandler<AddAppointmentGuestCommand, Appointment>
    {
        private readonly IDbSet<Appointment> _appointments;

        public AddAppointmentGuestCommandHandler(IDbSet<Appointment> appointments)
        {
            _appointments = appointments;
        }

        public Appointment Handle(AddAppointmentGuestCommand message)
        {
            var appt = _appointments
                .Include(x => x.GuestList)
                .FirstOrDefault(x => x.Id == message.AppointmentId);

            appt.GuestList.Add(new AppointmentGuest
            {
                UserId = message.UserId,
                AppointmentTimeSlotId = message.AppointmentTimeSlotId,
                Status = AppointmentGuestStatus.Pending
            });

            appt.Status = AppointmentStatus.PendingGuestConfirmation;

            return appt;
        }
    }

    public class AddAppointmentGuestCommandValidator : AbstractValidator<AddAppointmentGuestCommand>
    {

        public AddAppointmentGuestCommandValidator(IDbSet<Appointment> appointments, IDbSet<AppointmentGuest> appointmentGuests)
        {
            Custom(command =>
            {
                var exists = appointments.Any(appt =>appt.Id == command.AppointmentId);

                return !exists ? new ValidationFailure("AppointmentId", "This appointment does not exist") : null;
            });

            Custom(command =>
            {
                var isDuplicateGuest = appointments
                    .Include(x => x.GuestList)
                    .Any(appt =>
                        appt.Id == command.AppointmentId
                        && appt.GuestList.Any(guest =>
                            guest.UserId == command.UserId
                            && guest.AppointmentTimeSlotId == command.AppointmentTimeSlotId
                            )
                    );

                
                return isDuplicateGuest ? new ValidationFailure("UserId", "This user is already registered for this time slot") : null;
            });
        }
    }
}