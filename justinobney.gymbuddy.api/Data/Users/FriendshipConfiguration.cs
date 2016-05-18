using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace justinobney.gymbuddy.api.Data.Users
{
    public class FriendshipConfiguration : EntityTypeConfiguration<Friendship>
    {
        public FriendshipConfiguration()
        {
            Property(x => x.UserId)
                .IsRequired()
                .HasColumnAnnotation(
                    IndexAnnotation.AnnotationName,
                    new IndexAnnotation(
                        new IndexAttribute("IX_UserFriend", 1) { IsUnique = true }));

            Property(x => x.FriendId)
                .IsRequired()
                .HasColumnAnnotation(
                    IndexAnnotation.AnnotationName,
                    new IndexAnnotation(
                        new IndexAttribute("IX_UserFriend", 2) { IsUnique = true }));

            Property(x => x.FriendshipKey)
                .IsRequired();
        }
    }
}