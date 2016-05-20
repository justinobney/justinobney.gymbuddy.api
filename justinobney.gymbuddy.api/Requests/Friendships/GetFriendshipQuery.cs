using System.Data.Entity;
using System.Linq;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Enums;
using justinobney.gymbuddy.api.Interfaces;
using justinobney.gymbuddy.api.Requests.Decorators;
using MediatR;

namespace justinobney.gymbuddy.api.Requests.Friendships
{
    public class GetFriendshipQuery : IRequest<Friendship>, IFriendship
    {
        public long UserId { get; set; }
        public long FriendId { get; set; }
    }

    [DoNotValidate]
    [DoNotCommit]
    public class GetFriendshipQueryHandler : IRequestHandler<GetFriendshipQuery, Friendship>
    {
        private readonly IDbSet<Friendship> _friendships;

        public GetFriendshipQueryHandler(IDbSet<Friendship> friendships)
        {
            _friendships = friendships;
        }


        public Friendship Handle(GetFriendshipQuery message)
        {
            if (message.UserId == message.FriendId)
            {
                return new Friendship {Status = FriendshipStatus.Self};
            }

            var friendship = _friendships.FirstOrDefault(
                x =>
                    x.UserId == message.UserId
                    && x.FriendId == message.FriendId
                );

            return friendship ?? new Friendship { Status = FriendshipStatus.Unknown };
        }
    }
}