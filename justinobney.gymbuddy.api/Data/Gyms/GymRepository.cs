namespace justinobney.gymbuddy.api.Data.Gyms
{
    public class GymRepository : BaseRepository<Gym>
    {
        public GymRepository(AppContext context) : base(context)
        {
        }
    }
}