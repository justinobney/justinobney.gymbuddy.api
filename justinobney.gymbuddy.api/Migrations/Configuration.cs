using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using justinobney.gymbuddy.api.Data;
using justinobney.gymbuddy.api.Data.Devices;
using justinobney.gymbuddy.api.Data.Gyms;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Enums;

namespace justinobney.gymbuddy.api.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<Data.AppContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(AppContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.

            SeedUsers(context);
            SeedGyms(context);
        }
        
        private void SeedUsers(AppContext context)
        {
            context.Users.AddOrUpdate(
                u => u.Name,
                new User
                {
                    Devices = GetDeviceList(),
                    Name = "Tadpole User",
                    FitnessLevel = FitnessLevel.Tadpole,
                    Gender = Gender.Male,
                    FilterGender = GenderFilter.Both,
                    FilterFitnessLevel = FitnessLevel.Tadpole
                },
                new User
                {
                    Devices = GetDeviceList(),
                    Name = "Broette User",
                    FitnessLevel = FitnessLevel.Brotege,
                    Gender = Gender.Female,
                    FilterGender = GenderFilter.Both,
                    FilterFitnessLevel = FitnessLevel.Tadpole
                },
                new User
                {
                    Devices = GetDeviceList(),
                    Name = "Ladies Only Lady",
                    FitnessLevel = FitnessLevel.Tadpole,
                    Gender = Gender.Female,
                    FilterGender = GenderFilter.Female,
                    FilterFitnessLevel = FitnessLevel.Brotege
                }
            );
        }

        private void SeedGyms(AppContext context)
        {
            context.Gyms.AddOrUpdate(
                g => g.Name,
                new Gym
                {
                    Name = "GloboGym",
                    Zipcode = "70817",
                    CreatedAt = DateTime.UtcNow,
                    ModifiedAt = DateTime.UtcNow,
                    Members = context.Users.ToList()
                }
            );
        }

        private ICollection<Device> GetDeviceList()
        {
            return new List<Device> {new Device {DeviceId = Guid.NewGuid().ToString()}};
        }
    }
}
