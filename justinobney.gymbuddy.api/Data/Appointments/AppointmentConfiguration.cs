using System.Data.Entity.ModelConfiguration;

namespace justinobney.gymbuddy.api.Data.Appointments
{
    public class AppointmentConfiguration : EntityTypeConfiguration<Appointment>
    {
        public AppointmentConfiguration()
        {
            HasRequired(appt => appt.User)
                .WithMany()
                .WillCascadeOnDelete(false);
        }
    }
}