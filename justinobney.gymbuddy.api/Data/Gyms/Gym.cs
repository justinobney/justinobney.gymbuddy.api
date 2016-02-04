using System;
using System.Collections.Generic;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Interfaces;

namespace justinobney.gymbuddy.api.Data.Gyms
{
    public class Gym : IEntity, IHasTouchedProperties
    {
        public long Id { get; set; }

        public string Name { get; set; }
        public double? Lat { get; set; }
        public double? Lng { get; set; }
        public string Zipcode { get; set; }
        public List<User> Members { get; set; } = new List<User>(); 

        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}