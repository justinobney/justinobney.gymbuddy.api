using System.Data.Entity.ModelConfiguration;

namespace justinobney.gymbuddy.api.Data.Notifications
{
    public class NotificationConfiguration : EntityTypeConfiguration<Notification>
    {
        public NotificationConfiguration()
        {
            Property(x => x.Type)
                .IsRequired();
        }
    }
}