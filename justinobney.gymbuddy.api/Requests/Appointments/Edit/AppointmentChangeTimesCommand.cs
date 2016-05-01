using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
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

        public AppointmentChangeTimesCommandHandler(
            IDbSet<Appointment> appointments,
            IDbSet<AppointmentTimeSlot> timeslots,
            IDbSet<AppointmentGuest> guests,
            IDbSet<Device> devices 
            )
        {
            _appointments = appointments;
            _timeslots = timeslots;
            _guests = guests;
            _devices = devices;
        }

        public Appointment Handle(AppointmentChangeTimesCommand message)
        {
            var appointment = _appointments.First(x=>x.Id == message.AppointmentId);

            // for notifications
            var guestUserIds = _guests.Where(x => x.AppointmentId == message.AppointmentId).Select(x => x.UserId);
            message.Devices = _devices.Where(x => guestUserIds.Contains(x.UserId)).ToList();

            _timeslots.Where(x => x.AppointmentId == message.AppointmentId)
                .ToList()
                .ForEach(x => _timeslots.Remove(x));

            _guests.Where(x => x.AppointmentId == message.AppointmentId)
                .ToList()
                .ForEach(x => _guests.Remove(x));

            var timeslots = message.TimeSlots.Select(x => new AppointmentTimeSlot {Time = x}).ToList();

            appointment.TimeSlots = timeslots;
            appointment.ModifiedAt = DateTime.UtcNow;

            return appointment;
        }
    }
}