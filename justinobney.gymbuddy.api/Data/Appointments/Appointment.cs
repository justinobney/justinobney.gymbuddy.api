using System;
using System.Collections.Generic;
using justinobney.gymbuddy.api.Data.Gyms;
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
        public string Title { get; set; }
        public string Description { get; set; }
        public AppointmentStatus Status { get; set; }
        public DateTime? ConfirmedTime { get; set; }
        
        public ICollection<AppointmentGuest> GuestList { get; set; } = new List<AppointmentGuest>();
        public ICollection<AppointmentTimeSlot> TimeSlots { get; set; } = new List<AppointmentTimeSlot>();
        public ICollection<AppointmentComment> Comments { get; set; } = new List<AppointmentComment>();
        public ICollection<AppointmentExercise> Exercises { get; set; } = new List<AppointmentExercise>();
        public ICollection<AppointmentKudos> Kudos { get; set; }  = new List<AppointmentKudos>();

        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public User User { get; set; }
        public Gym Gym { get; set; }
    }

    public class AppointmentKudos : IEntity
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long AppointmentId { get; set; }
    }
}