namespace justinobney.gymbuddy.api.Data.Appointments
{
    public class AppointmentRepository : BaseRepository<Appointment>
    {
        public AppointmentRepository(AppContext context) : base(context)
        {
        }
    }
}