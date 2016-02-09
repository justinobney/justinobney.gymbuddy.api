using System;
using System.Collections.Generic;
using justinobney.gymbuddy.api.Data.Appointments;
using justinobney.gymbuddy.api.Data.Devices;
using justinobney.gymbuddy.api.Data.Gyms;
using justinobney.gymbuddy.api.Enums;
using justinobney.gymbuddy.api.Interfaces;

namespace justinobney.gymbuddy.api.Data.Users
{
    public class User :IEntity, IHasTouchedProperties
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public FitnessLevel FitnessLevel { get; set; }
        public FitnessLevel FilterFitnessLevel { get; set; }
        public Gender Gender { get; set; }
        public GenderFilter FilterGender { get; set; }

        public List<Device> Devices { get; set; } = new List<Device>();
        public List<Gym> Gyms { get; set; } = new List<Gym>();

        public virtual List<Appointment> Appointments { get; set; }

        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}