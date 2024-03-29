﻿using FluentValidation;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Responses;
using MediatR;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace justinobney.gymbuddy.api.Requests.Friendships
{
    public class GetFriendshipsByFacebookIdsQuery : IRequest<ICollection<FacebookFriendshipListing>>
    {
        public long UserId { get; set; }
        public ICollection<string> FbIds { get; set; }
    }

    public class GetUserListByFacebookIdsQueryHandler : IRequestHandler<GetFriendshipsByFacebookIdsQuery, ICollection<FacebookFriendshipListing>>
    {
        private readonly IDbSet<User> _users;
        private readonly IDbSet<Friendship> _friendships;

        public GetUserListByFacebookIdsQueryHandler(IDbSet<User> users, IDbSet<Friendship> friendships)
        {
            _users = users;
            _friendships = friendships;
        }

        public ICollection<FacebookFriendshipListing> Handle(GetFriendshipsByFacebookIdsQuery message)
        {
            var facebookFriendshipListings = new List<FacebookFriendshipListing>();

            var users = _users.Where(x => !string.IsNullOrEmpty(x.FacebookUserId) && message.FbIds.Contains(x.FacebookUserId)).ToList();
            var userFacebookIds = users.Select(x => x.FacebookUserId).Distinct().ToList();
            var userIds = users.Select(y => y.Id).Distinct().ToList();

            //existing users
            var existingFriendships = _friendships.Where(x => userIds.Contains(x.FriendId) && x.UserId == message.UserId).ToList();
            foreach (var user in users)
            {
                var facebookFriendshipListing = new FacebookFriendshipListing { UserId = user.Id, FacebookUserId = user.FacebookUserId };

                var existingFriendship = existingFriendships.FirstOrDefault(x => x.FriendId == user.Id);
                if (facebookFriendshipListing.UserId == message.UserId)
                    facebookFriendshipListing.Status = Enums.FacebookFriendshipStatus.Self;
                else if (existingFriendship != null && existingFriendship.Status == Enums.FriendshipStatus.Active)
                    facebookFriendshipListing.Status = Enums.FacebookFriendshipStatus.SquadUser;
                else if (existingFriendship != null && existingFriendship.Status == Enums.FriendshipStatus.Pending)
                    facebookFriendshipListing.Status = Enums.FacebookFriendshipStatus.PendingSquadUser;
                else
                    facebookFriendshipListing.Status = Enums.FacebookFriendshipStatus.NonSquadUser;

                facebookFriendshipListings.Add(facebookFriendshipListing);
            }

            //non-existing users
            var nonMemberFriendFacebookIds = message.FbIds.Except(userFacebookIds).ToList();
            foreach (var nonMemberFriendFacebookId in nonMemberFriendFacebookIds)
            {
                facebookFriendshipListings.Add(new FacebookFriendshipListing
                {
                    FacebookUserId = nonMemberFriendFacebookId,
                    Status = Enums.FacebookFriendshipStatus.UnknownFacebookFriend
                });
            }

            return facebookFriendshipListings;
        }
    }

    public class GetUserListByFacebookIdsQueryValidator : AbstractValidator<GetFriendshipsByFacebookIdsQuery>
    {
        public GetUserListByFacebookIdsQueryValidator()
        {
            RuleFor(x => x.FbIds).NotNull();
        }
    }
}