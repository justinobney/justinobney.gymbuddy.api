using System.Data.Entity;
using System.Linq;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Enums;
using justinobney.gymbuddy.api.Interfaces;
using justinobney.gymbuddy.api.Requests.Decorators;
using MediatR;

namespace justinobney.gymbuddy.api.Requests.Frienships
{
    public class ConfirmFriendshipCommand : IRequest<Friendship>, IFriendship
    {
        public long UserId { get; set; }
        public long FriendId { get; set; }
    }

    [DoNotValidate]
    public class ConfirmFriendshipCommandHandler : IRequestHandler<ConfirmFriendshipCommand, Friendship>
    {
        private readonly IDbSet<Friendship> _friendships;

        public ConfirmFriendshipCommandHandler(IDbSet<Friendship> friendships)
        {
            _friendships = friendships;
        }

        public Friendship Handle(ConfirmFriendshipCommand message)
        {
            var friendshipKey = Friendship.GetFriendshipKey(message);

            _friendships.Where(x => x.FriendshipKey == friendshipKey)
                .ToList()
                .ForEach(x =>
                {
                    x.Status = FriendshipStatus.Active;
                });

            return _friendships.First(
                x =>
                    x.FriendshipKey == friendshipKey
                    && x.UserId == message.UserId
                );
        }
    }
}