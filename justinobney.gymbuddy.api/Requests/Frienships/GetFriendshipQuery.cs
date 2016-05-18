using System.Data.Entity;
using System.Linq;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Interfaces;
using justinobney.gymbuddy.api.Requests.Decorators;
using MediatR;

namespace justinobney.gymbuddy.api.Requests.Frienships
{
    public class GetFriendshipQuery : IRequest<Friendship>, IFriendship
    {
        public long UserId { get; set; }
        public long FriendId { get; set; }
    }

    [DoNotValidate]
    public class GetFriendshipQueryHandler : IRequestHandler<GetFriendshipQuery, Friendship>
    {
        private readonly IDbSet<Friendship> _friendships;

        public GetFriendshipQueryHandler(IDbSet<Friendship> friendships)
        {
            _friendships = friendships;
        }


        public Friendship Handle(GetFriendshipQuery message)
        {
            return _friendships.First(
                x =>
                    x.UserId == message.UserId
                    && x.FriendId == message.FriendId
                );
        }
    }
}