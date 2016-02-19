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
    internal sealed class Configuration : DbMigrationsConfiguration<AppContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(AppContext context)
        {
            var gym = new Gym
            {
                Name = "GloboGym",
                Zipcode = "70817",
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow
            };

            context.Set<User>()
                .AddOrUpdate(
                    u => u.Name,
                    new User
                    {
                        Devices = GetDeviceList(),
                        Name = "Tadpole User",
                        FitnessLevel = FitnessLevel.Tadpole,
                        Gender = Gender.Male,
                        FilterGender = GenderFilter.Both,
                        FilterFitnessLevel = FitnessLevel.Tadpole,
                        Gyms = new List<Gym> { gym }
                    },
                    new User
                    {
                        Devices = GetDeviceList(),
                        Name = "Broette User",
                        FitnessLevel = FitnessLevel.Brotege,
                        Gender = Gender.Female,
                        FilterGender = GenderFilter.Both,
                        FilterFitnessLevel = FitnessLevel.Tadpole,
                        Gyms = new List<Gym> { gym }
                    },
                    new User
                    {
                        Devices = GetDeviceList(),
                        Name = "Ladies Only Lady",
                        FitnessLevel = FitnessLevel.Tadpole,
                        Gender = Gender.Female,
                        FilterGender = GenderFilter.Female,
                        FilterFitnessLevel = FitnessLevel.Brotege,
                        Gyms = new List<Gym> { gym }
                    }
                );
        }
        
        private ICollection<Device> GetDeviceList()
        {
            return new List<Device> {new Device {DeviceId = Guid.NewGuid().ToString()}};
        }
    }
}
