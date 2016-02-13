using System;

namespace justinobney.gymbuddy.api.Data.Appointments
{
    public class AppointmentTimeSlot
    {
        public long Id { get; set; }
        public long AppointmentId { get; set; }
        public DateTime? Time { get; set; }
    }
}