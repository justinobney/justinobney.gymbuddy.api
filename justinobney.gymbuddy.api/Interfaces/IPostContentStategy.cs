using justinobney.gymbuddy.api.Data.AsyncJobs;
using justinobney.gymbuddy.api.Requests.Posts;

namespace justinobney.gymbuddy.api.Interfaces
{
    public interface IPostContentStategy
    {
        AsyncJob Handle(CreatePostCommand message);
    }
}