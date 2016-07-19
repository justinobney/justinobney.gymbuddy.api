using System.Data.Entity.ModelConfiguration;
using System.Web.Http.Results;
using justinobney.gymbuddy.api.Enums;
using justinobney.gymbuddy.api.Interfaces;

namespace justinobney.gymbuddy.api.Data.AsyncJobs
{
    public class AsyncJob : IEntity
    {
        public long Id { get; set; }
        public JobStatus Status { get; set; }
        public string StatusUrl { get; set; }
        public string ContentUrl { get; set; }
    }

    public class AsyncJobConfiguration : EntityTypeConfiguration<AsyncJob>
    {
    }
}