using System.Data.Entity;
using justinobney.gymbuddy.api.Data.Devices;
using justinobney.gymbuddy.api.Data.Gyms;
using justinobney.gymbuddy.api.Data.Users;

namespace justinobney.gymbuddy.api.Data
{
    public class AppContext : DbContext
    {   
        public AppContext() : base("name=AppContext")
        {
            Configuration.LazyLoadingEnabled = true;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.AddFromAssembly(typeof(AppContext).Assembly);
        }

        //This is here only for unit testing purposes
        public virtual void SetModified(object entity)
        {
            Entry(entity).State = EntityState.Modified;
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<Gym> Gyms { get; set; }
    }
}
