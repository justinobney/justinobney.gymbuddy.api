using System;
using System.Linq;
using justinobney.gymbuddy.api.Enums;
using justinobney.gymbuddy.api.Interfaces;

namespace justinobney.gymbuddy.api.Data.Users
{
    public class Friendship : IEntity, IFriendship
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long FriendId { get; set; }
        public string FriendshipKey { get; set; }
        public FriendshipStatus Status { get; set; }
        public DateTime? FriendshipDate { get; set; }

        public string StatusText => Status.ToString();
        public bool Initiator { get; set; }
        public User Friend { get; set; }

        public static string GetFriendshipKey(IFriendship friendship)
        {
            var ids = new[] { friendship.UserId, friendship.FriendId };
            return $"{ids.Min()}||{ids.Max()}";
        }
    }
}