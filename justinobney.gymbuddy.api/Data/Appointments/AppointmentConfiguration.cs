using System.Data.Entity.ModelConfiguration;

namespace justinobney.gymbuddy.api.Data.Appointments
{
    public class AppointmentConfiguration : EntityTypeConfiguration<Appointment>
    {
        public AppointmentConfiguration()
        {

        }
    }

    public class AppointmentGuestConfiguration : EntityTypeConfiguration<AppointmentGuest>
    {
        public AppointmentGuestConfiguration()
        {
            HasRequired(appt => appt.User)
                .WithMany()
                .WillCascadeOnDelete(false);

            HasRequired(appt => appt.TimeSlot)
                .WithMany()
                .WillCascadeOnDelete(false);
        }
    }
}