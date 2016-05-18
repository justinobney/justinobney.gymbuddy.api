using System.Data.Entity;
using System.Linq;
using FluentValidation;
using FluentValidation.Results;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Enums;
using justinobney.gymbuddy.api.Interfaces;
using MediatR;

namespace justinobney.gymbuddy.api.Requests.Frienships
{
    public class RequestFriendshipCommand : IRequest<Friendship>, IFriendship
    {
        public long UserId { get; set; }
        public long FriendId { get; set; }
    }

    public class RequestFriendshipCommandHandler : IRequestHandler<RequestFriendshipCommand, Friendship>
    {
        private readonly IDbSet<Friendship> _friendships;

        public RequestFriendshipCommandHandler(IDbSet<Friendship> friendships)
        {
            _friendships = friendships;
        }

        public Friendship Handle(RequestFriendshipCommand message)
        {
            var friendshipKey = Friendship.GetFriendshipKey(message);

            var userToFriend = new Friendship
            {
                UserId = message.UserId,
                FriendId = message.FriendId,
                FriendshipKey = friendshipKey,
                Status = FriendshipStatus.Pending
            };

            var friendToUser = new Friendship
            {
                UserId = message.FriendId,
                FriendId = message.UserId,
                FriendshipKey = friendshipKey,
                Status = FriendshipStatus.Pending
            };

            _friendships.Add(userToFriend);
            _friendships.Add(friendToUser);

            return userToFriend;
        }
    }

    public class RequestFriendshipValidator : AbstractValidator<RequestFriendshipCommand>
    {
        private readonly IDbSet<Friendship> _friendships;

        public RequestFriendshipValidator(IDbSet<Friendship> friendships)
        {
            _friendships = friendships;

            Custom(command =>
            {
                var friendshipKey = Friendship.GetFriendshipKey(command);

                return _friendships.Any(x => x.FriendshipKey == friendshipKey)
                    ? new ValidationFailure("UserId", "Can not request existing friendship")
                    : null;
            });
        }
    }
}