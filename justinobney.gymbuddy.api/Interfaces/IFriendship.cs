namespace justinobney.gymbuddy.api.Interfaces
{
    public interface IFriendship
    {
        long UserId { get; set; }
        long FriendId { get; set; }
    }
}