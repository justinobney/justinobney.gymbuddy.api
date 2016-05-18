using System;
using justinobney.gymbuddy.api.Enums;
using justinobney.gymbuddy.api.Interfaces;

namespace justinobney.gymbuddy.api.Data.Users
{
    public class Friendship : IEntity
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long FriendId { get; set; }
        public string FriendshipKey { get; set; }
        public FriendshipStatus Status { get; set; }
        public DateTime? FriendshipDate { get; set; }
    }
}