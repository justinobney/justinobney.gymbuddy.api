using System.Collections.Generic;
using System.Threading.Tasks;
using justinobney.gymbuddy.api.Interfaces;
using justinobney.gymbuddy.api.Requests.Decorators;
using MediatR;
using Stream;

namespace justinobney.gymbuddy.api.Requests.Posts
{
    public class GetUserActivityQuery : IAsyncRequest<IEnumerable<Activity>>
    {
        public string UserId { get; set; }
        public string LastId { get; set; }
    }

    [DoNotValidate]
    [DoNotCommit]
    public class GetUserActivityQueryHandler : IAsyncRequestHandler<GetUserActivityQuery, IEnumerable<Activity>>
    {
        private readonly IStreamClientProxy _streamClientProxy;


        public GetUserActivityQueryHandler(IStreamClientProxy streamClientProxy)
        {
            _streamClientProxy = streamClientProxy;
        }


        public async Task<IEnumerable<Activity>> Handle(GetUserActivityQuery message)
        {
            return await _streamClientProxy.GetActivity(message.UserId, message.LastId);
        }
    }
}