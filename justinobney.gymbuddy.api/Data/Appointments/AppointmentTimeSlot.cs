using System;
using justinobney.gymbuddy.api.Interfaces;

namespace justinobney.gymbuddy.api.Data.Appointments
{
    public class AppointmentTimeSlot :IEntity
    {
        public long Id { get; set; }
        public long AppointmentId { get; set; }
        public DateTime? Time { get; set; }
    }
}