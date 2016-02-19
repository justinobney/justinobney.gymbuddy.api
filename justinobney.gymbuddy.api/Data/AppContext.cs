using System.Data.Entity;

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
            modelBuilder.Configurations.AddFromAssembly(assembly: typeof(AppContext).Assembly);
        }

        //This is here only for unit testing purposes
        public virtual void SetModified(object entity)
        {
            Entry(entity).State = EntityState.Modified;
        }
    }
}
