namespace justinobney.gymbuddy.api.Data.Users
{
    public class UserRepository : BaseRepository<User>
    {
        public UserRepository(AppContext context) : base(context)
        {
        }
    }
}