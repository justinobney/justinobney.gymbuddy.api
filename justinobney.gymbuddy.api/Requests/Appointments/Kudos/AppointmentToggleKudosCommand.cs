using System.Data.Entity;
using System.Linq;
using justinobney.gymbuddy.api.Data.Appointments;
using justinobney.gymbuddy.api.Requests.Decorators;
using MediatR;

namespace justinobney.gymbuddy.api.Requests.Appointments.Kudos
{
    public class AppointmentToggleKudosCommand : IRequest<Appointment>
    {
        public long AppointmentId { get; set; }
        public long UserId { get; set; }
    }

    [DoNotValidate]
    public class AppointmentToggleKudosCommandHandler : IRequestHandler<AppointmentToggleKudosCommand, Appointment>
    {
        private readonly IDbSet<AppointmentKudos> _kudoses;
        private readonly IDbSet<Appointment> _appointments;

        public AppointmentToggleKudosCommandHandler(IDbSet<Appointment> appointments, IDbSet<AppointmentKudos> kudoses)
        {
            _kudoses = kudoses;
            _appointments = appointments;
        }

        public Appointment Handle(AppointmentToggleKudosCommand message)
        {
            var appt = _appointments
                .Include(x => x.Kudos)
                .First(x => x.Id == message.AppointmentId);

            var userKudos = _kudoses.FirstOrDefault(x =>
                x.UserId == message.UserId
                && x.AppointmentId == message.AppointmentId
            );

            if (userKudos == null)
            {
                _kudoses.Add(new AppointmentKudos
                {
                    UserId = message.UserId,
                    AppointmentId = message.AppointmentId
                });
            }
            else
            {
                _kudoses.Remove(userKudos);
            }

            return appt;
        }
    }
}