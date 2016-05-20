using System.Data.Entity;
using System.Linq;
using FluentValidation;
using FluentValidation.Results;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Enums;
using justinobney.gymbuddy.api.Interfaces;
using MediatR;

namespace justinobney.gymbuddy.api.Requests.Friendships
{
    public class ConfirmFriendshipCommand : IRequest<Friendship>, IFriendship
    {
        public long UserId { get; set; }
        public long FriendId { get; set; }
    }

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

    public class ConfirmFriendshipValidator : AbstractValidator<ConfirmFriendshipCommand>
    {
        private readonly IDbSet<Friendship> _friendships;

        public ConfirmFriendshipValidator(IDbSet<Friendship> friendships)
        {
            _friendships = friendships;

            Custom(command =>
            {
                var isConfirmFromInitiator = _friendships.Any(
                    x =>
                        x.UserId == command.UserId
                        && x.FriendId == command.FriendId
                        && x.Initiator == true
                    );
                return isConfirmFromInitiator ?  new ValidationFailure("UserId", "You can not confirm a request to friend someone else") : null;
            });
        }
    }
}