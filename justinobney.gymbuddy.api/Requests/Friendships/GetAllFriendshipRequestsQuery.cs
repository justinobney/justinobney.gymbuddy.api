using System.Data.Entity;
using System.Linq;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Enums;
using justinobney.gymbuddy.api.Requests.Decorators;
using MediatR;

namespace justinobney.gymbuddy.api.Requests.Friendships
{
    public class GetAllFriendshipRequestsQuery : IRequest<IQueryable<Friendship>>
    {
        public long UserId { get; set; }
    }

    [DoNotValidate]
    [DoNotCommit]
    public class GetAllFriendshipRequestsQueryHandler : IRequestHandler<GetAllFriendshipRequestsQuery, IQueryable<Friendship>>
    {
        private readonly IDbSet<Friendship> _friendships;

        public GetAllFriendshipRequestsQueryHandler(IDbSet<Friendship> friendships)
        {
            _friendships = friendships;
        }


        public IQueryable<Friendship> Handle(GetAllFriendshipRequestsQuery message)
        {
            return _friendships.Where(
                x =>
                    x.UserId == message.UserId
                    && x.Status == FriendshipStatus.Pending
                    && x.Initiator == false
                );
        }
    }
}