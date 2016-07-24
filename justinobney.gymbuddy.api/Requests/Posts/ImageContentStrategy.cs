using System.Data.Entity;
using System.Linq;
using Hangfire;
using justinobney.gymbuddy.api.Data;
using justinobney.gymbuddy.api.Data.AsyncJobs;
using justinobney.gymbuddy.api.Data.Posts;
using justinobney.gymbuddy.api.Enums;
using justinobney.gymbuddy.api.Interfaces;
using Serilog;
using WebGrease.Css.Extensions;

namespace justinobney.gymbuddy.api.Requests.Posts
{
    public class ImageContentStrategy : IPostContentStategy
    {
        private readonly ILogger _log;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IDbSet<AsyncJob> _asyncJobs;
        private readonly IDbSet<Post> _posts;
        private readonly IImageUploader _imageUploader;
        private readonly AppContext _context;

        public ImageContentStrategy(
            ILogger log,
            IBackgroundJobClient backgroundJobClient,
            IDbSet<AsyncJob> asyncJobs,
            IDbSet<Post> posts,
            IImageUploader imageUploader,
            AppContext context
            )
        {
            _log = log;
            _backgroundJobClient = backgroundJobClient;
            _asyncJobs = asyncJobs;
            _posts = posts;
            _imageUploader = imageUploader;
            _context = context;
        }

        public AsyncJob Handle(CreatePostCommand message)
        {
            _log.Information("Scheduling Image Upload Processing");

            var job = new AsyncJob
            {
                Status = JobStatus.Pending
            };

            _asyncJobs.Add(job);
            _context.SaveChanges();

            job.StatusUrl = $"/api/jobs/{job.Id}";

            _backgroundJobClient.Enqueue(() => ProcessImageContent(message, job.Id));
            return job;
        }

        public void ProcessImageContent(CreatePostCommand command, long jobId)
        {
            _log.Information($"Processing Image Upload: {jobId}");
            var theJob = _asyncJobs.FirstOrDefault(x => x.Id == jobId);

            if (theJob == null)
            {
                _log.Information($"Processing Image Upload::: Unable to find job - {jobId}");
            }
            else
            {
                command.Content.Where(x=>x.Type == PostType.Image).ForEach(x =>
                {
                    var url = _imageUploader.UploadFromDataUri(x.Value);
                    x.Value = url;
                });

                var post = new Post
                {
                    Contents = command.Content,
                    UserId = command.UserId
                };

                _posts.Add(post);
                _context.SaveChanges();
                
                theJob.Status = JobStatus.Complete;
                theJob.ContentUrl = $"/api/posts/{post.Id}";
                _context.SaveChanges();
            }
        }
    }
}