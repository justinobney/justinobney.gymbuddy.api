using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace justinobney.gymbuddy.api.Data.Users
{
    public class UserConfiguration : EntityTypeConfiguration<User>
    {
        public UserConfiguration()
        {
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