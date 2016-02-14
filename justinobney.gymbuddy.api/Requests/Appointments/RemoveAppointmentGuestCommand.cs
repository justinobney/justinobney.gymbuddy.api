using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using justinobney.gymbuddy.api.Data;
using justinobney.gymbuddy.api.Data.Appointments;
using justinobney.gymbuddy.api.Enums;
using justinobney.gymbuddy.api.Requests.Decorators;
using MediatR;

namespace justinobney.gymbuddy.api.Requests.Appointments
{
    public class RemoveAppointmentGuestCommand : IAsyncRequest<Appointment>
    {
        public long GuestAppointmentId { get; set; }
    }

    [DoNotValidate]
    public class RemoveAppointmentGuestCommandHandler : IAsyncRequestHandler<RemoveAppointmentGuestCommand, Appointment>
    {
        private readonly AppointmentRepository _apptRepo;
        private readonly AppContext _context;

        public RemoveAppointmentGuestCommandHandler(AppointmentRepository apptRepo, AppContext context)
        {
            _apptRepo = apptRepo;
            _context = context;
        }

        public async Task<Appointment> Handle(RemoveAppointmentGuestCommand message)
        {
            var guestAppt = await _context.AppointmentGuests.FindAsync(message.GuestAppointmentId);
            _context.AppointmentGuests.Remove(guestAppt);
            _context.SaveChanges();


            var appt = await _apptRepo.Find(appointment => appointment.Id == guestAppt.AppointmentId)
                .Include(x => x.GuestList)
                .FirstOrDefaultAsync();

            return appt;
        }
    }
}