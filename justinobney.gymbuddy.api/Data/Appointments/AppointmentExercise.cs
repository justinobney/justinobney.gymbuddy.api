using justinobney.gymbuddy.api.Interfaces;

namespace justinobney.gymbuddy.api.Data.Appointments
{
    public class AppointmentExercise : IEntity
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long AppointmentId { get; set; }
    }
}