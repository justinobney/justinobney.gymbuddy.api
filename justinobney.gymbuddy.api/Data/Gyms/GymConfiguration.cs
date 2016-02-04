using System.Data.Entity.ModelConfiguration;

namespace justinobney.gymbuddy.api.Data.Gyms
{
    public class GymConfiguration : EntityTypeConfiguration<Gym>
    {
        public GymConfiguration()
        {
            Property(x => x.Name)
                .IsRequired();
        }
    }
}