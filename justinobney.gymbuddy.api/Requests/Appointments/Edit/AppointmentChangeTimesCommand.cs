using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using justinobney.gymbuddy.api.Data;
using justinobney.gymbuddy.api.Data.Appointments;
using justinobney.gymbuddy.api.Data.Devices;
using MediatR;

namespace justinobney.gymbuddy.api.Requests.Appointments.Edit
{
    public class AppointmentChangeTimesCommand : IRequest<Appointment>
    {
        public long AppointmentId { get; set; }
        public long UserId { get; set; }
        public List<DateTime?> TimeSlots { get; set; } = new List<DateTime?>();
        public IEnumerable<Device> Devices { get; set; }
    }

    public class AppointmentChangeTimesCommandHandler : IRequestHandler<AppointmentChangeTimesCommand, Appointment>
    {
        private readonly IDbSet<Appointment> _appointments;
        private readonly IDbSet<AppointmentTimeSlot> _timeslots;
        private readonly IDbSet<AppointmentGuest> _guests;
        private readonly IDbSet<Device> _devices;
        private readonly AppContext _context;

        public AppointmentChangeTimesCommandHandler(
            IDbSet<Appointment> appointments,
            IDbSet<AppointmentTimeSlot> timeslots,
            IDbSet<AppointmentGuest> guests,
            IDbSet<Device> devices,
            AppContext context
            )
        {
            _appointments = appointments;
            _timeslots = timeslots;
            _guests = guests;
            _devices = devices;
            _context = context;
        }

        public Appointment Handle(AppointmentChangeTimesCommand message)
        {
            var appointment = _appointments.First(x=>x.Id == message.AppointmentId);

            // for notifications
            // TODO: this can go away since we are putting the guests back on
            var guestUserIds = _guests.Where(x => x.AppointmentId == message.AppointmentId).Select(x => x.UserId);
            message.Devices = _devices.Where(x => guestUserIds.Contains(x.UserId)).ToList();


            var timeslot = new AppointmentTimeSlot
            {
                Time = message.TimeSlots.First(),
                AppointmentId = message.AppointmentId
            };

            var newGuests = new List<AppointmentGuest>();

            _timeslots.Where(x => x.AppointmentId == message.AppointmentId)
                .ToList()
                .ForEach(x => _timeslots.Remove(x));
            
            _guests.Where(x => x.AppointmentId == message.AppointmentId)
                .ToList()
                .ForEach(x =>
                {
                    _guests.Remove(x);
                    newGuests.Add(new AppointmentGuest
                    {
                        UserId = x.UserId,
                        AppointmentId = x.AppointmentId,
                        Status = x.Status
                    });
                });

            appointment.ModifiedAt = DateTime.UtcNow;
            _timeslots.Add(timeslot);

            _context.SaveChanges();

            newGuests.ForEach(x =>
            {
                x.AppointmentTimeSlotId = timeslot.Id;
                _guests.Add(x);
            });

            return appointment;
        }
    }
}