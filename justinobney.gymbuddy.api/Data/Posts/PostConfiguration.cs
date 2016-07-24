using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace justinobney.gymbuddy.api.Data.Posts
{
    public class PostConfiguration : EntityTypeConfiguration<Post>
    {
        public PostConfiguration()
        {
            Property(x => x.UserId)
                .IsRequired();
        }
    }

    public class PostKudosConfiguration : EntityTypeConfiguration<PostKudos>
    {
        public PostKudosConfiguration()
        {
            Property(x => x.UserId)
                .IsRequired()
                .HasColumnAnnotation(
                    IndexAnnotation.AnnotationName,
                    new IndexAnnotation(
                        new IndexAttribute("IX_UserPostKudos", 1) { IsUnique = true }));

            Property(x => x.PostId)
                .IsRequired()
                .HasColumnAnnotation(
                    IndexAnnotation.AnnotationName,
                    new IndexAnnotation(
                        new IndexAttribute("IX_UserPostKudos", 2) { IsUnique = true }));
        }
    }
}