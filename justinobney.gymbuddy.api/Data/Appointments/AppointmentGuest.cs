using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Enums;
using justinobney.gymbuddy.api.Interfaces;

namespace justinobney.gymbuddy.api.Data.Appointments
{
    public class AppointmentGuest : IEntity
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long AppointmentId { get; set; }
        public long AppointmentTimeSlotId { get; set; }

        public AppointmentTimeSlot TimeSlot { get; set; }

        public AppointmentGuestStatus Status { get; set; }

        public User User { get; set; }
    }
}