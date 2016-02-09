using System;
using System.Collections.Generic;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Enums;
using justinobney.gymbuddy.api.Interfaces;

namespace justinobney.gymbuddy.api.Data.Appointments
{
    public class Appointment : IEntity, IHasTouchedProperties
    {
        public long Id { get; set; }

        public long UserId { get; set; }
        public long? GymId { get; set; }
        public string Location { get; set; }
        public AppointmentStatus Status { get; set; }
        public DateTime? ConfirmedTime { get; set; }
        
        public List<User> GuestList { get; set; }
        public List<AppointmentTimeSlot> TimeSlots { get; set; }

        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }

    public class AppointmentTimeSlot
    {
        public long Id { get; set; }
        public long AppointmentId { get; set; }
        public DateTime? Time { get; set; }
    }
}