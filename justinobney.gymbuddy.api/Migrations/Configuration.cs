using System;
using System.Data.Entity.Migrations;
using justinobney.gymbuddy.api.Data;
using justinobney.gymbuddy.api.Data.Gyms;

namespace justinobney.gymbuddy.api.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<AppContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(AppContext context)
        {

            context.Set<Gym>()
                .AddOrUpdate(
                    x => x.Name,
                    new Gym
                    {
                        Name = "LA Fitness - Siegen",
                        Zipcode = "70817",
                        CreatedAt = DateTime.UtcNow,
                        ModifiedAt = DateTime.UtcNow
                    },
                    new Gym
                    {
                        Name = "LA Fitness - Perkins Rowe",
                        Zipcode = "70817",
                        CreatedAt = DateTime.UtcNow,
                        ModifiedAt = DateTime.UtcNow
                    }
                );

        }
    }
}
