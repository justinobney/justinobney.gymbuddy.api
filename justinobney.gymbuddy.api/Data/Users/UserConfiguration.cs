using System.Data.Entity.ModelConfiguration;

namespace justinobney.gymbuddy.api.Data.Users
{
    public class UserConfiguration : EntityTypeConfiguration<User>
    {
        public UserConfiguration()
        {

            HasMany(u => u.Appointments)
                .WithMany(a => a.GuestList)
                .Map(configuration =>
                {
                    configuration.MapLeftKey("User_Id");
                    configuration.MapRightKey("Appointment_Id");
                    configuration.ToTable("UserAppointments");
                });
            //Property(x => x.Email)
            //    .IsRequired()
            //    .HasMaxLength(100)
            //    .HasColumnAnnotation(
            //        IndexAnnotation.AnnotationName,
            //        new IndexAnnotation(
            //            new IndexAttribute("IX_UserNameEmail", 1) { IsUnique = true }));

            //Property(x => x.Username)
            //    .IsRequired()
            //    .HasMaxLength(100)
            //    .HasColumnAnnotation(
            //        IndexAnnotation.AnnotationName,
            //        new IndexAnnotation(
            //            new IndexAttribute("IX_UserNameEmail", 2) { IsUnique = true }));
        }
    }
}