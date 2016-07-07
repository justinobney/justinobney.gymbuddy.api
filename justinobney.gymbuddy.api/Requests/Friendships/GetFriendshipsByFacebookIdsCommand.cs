using FluentValidation;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Responses;
using MediatR;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace justinobney.gymbuddy.api.Requests.Friendships
{
    public class GetFriendshipsByFacebookIdsCommand : IRequest<IList<ProfileFriendshipListing>>
    {
        public long UserId { get; set; }
        public IList<string> FbIds { get; set; }
    }

    public class GetUserListByFacebookIdsCommandHandler : IRequestHandler<GetFriendshipsByFacebookIdsCommand, IList<ProfileFriendshipListing>>
    {
        private readonly IDbSet<User> _users;
        private readonly IDbSet<Friendship> _friendships;

        public GetUserListByFacebookIdsCommandHandler(IDbSet<User> users, IDbSet<Friendship> friendships)
        {
            _users = users;
            _friendships = friendships;
        }

        public IList<ProfileFriendshipListing> Handle(GetFriendshipsByFacebookIdsCommand message)
        {
            var users = _users.Where(x => !string.IsNullOrEmpty(x.FacebookUserId) && message.FbIds.Contains(x.FacebookUserId)).ToList();
            var userIds = users.Select(y => y.Id).Distinct().ToList();
            var existingFriendships = _friendships.Where(x => userIds.Contains(x.FriendId) && x.UserId == message.UserId).ToList();
            var profiles = MappingConfig.Instance.Map<List<ProfileFriendshipListing>>(users);
            foreach (var profileFriendshipListing in profiles)
            {
                var existingFriendship = existingFriendships.FirstOrDefault(x => x.FriendId == profileFriendshipListing.Id);
                profileFriendshipListing.Status = existingFriendship?.Status ?? ((profileFriendshipListing.Id == message.UserId) ? Enums.FriendshipStatus.Self : Enums.FriendshipStatus.Unknown);
            }
            return profiles;
        }
    }

    public class GetUserListByFacebookIdsCommandValidator : AbstractValidator<GetFriendshipsByFacebookIdsCommand>
    {
        public GetUserListByFacebookIdsCommandValidator()
        {
            RuleFor(x => x.FbIds).NotNull();
        }
    }
}