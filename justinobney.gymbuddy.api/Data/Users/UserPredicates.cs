using System;
using System.Linq;
using System.Linq.Expressions;
using justinobney.gymbuddy.api.Enums;

namespace justinobney.gymbuddy.api.Data.Users
{
    public class UserPredicates
    {
        public static Expression<Func<User, bool>> RestrictMember(User currentUser, long gymId)
        {
            Expression<Func<User, bool>> predicate = u => u.Gyms.Any(g => g.Id == gymId)
                                                          && u.Id != currentUser.Id;

            predicate = predicate.And(RestrictMemberByFitnessLevel(currentUser));
            predicate = predicate.And(RestrictMemberByGender(currentUser));

            return predicate;
        }

        public static Expression<Func<User, bool>> RestrictMemberByGender(User currentUser)
        {
            return user => (
                ((int) currentUser.FilterGender == (int) user.Gender &&
                 (int) currentUser.Gender == (int) user.FilterGender)
                ||
                (currentUser.FilterGender == GenderFilter.Both &&
                 (int) user.FilterGender == (int) currentUser.Gender)
                ||
                (user.FilterGender == GenderFilter.Both &&
                 (int) currentUser.FilterGender == (int) user.Gender)
                ||
                (user.FilterGender == GenderFilter.Both &&
                 currentUser.FilterGender == GenderFilter.Both)
                );
        }

        public static Expression<Func<User, bool>> RestrictMemberByFitnessLevel(User currentUser)
        {
            return user => user.FilterFitnessLevel <= currentUser.FitnessLevel
                        && user.FitnessLevel >= currentUser.FilterFitnessLevel;
        }

        public static Expression<Func<User, bool>> RestrictByDeviceId(string deviceId)
        {
            return u => u.Devices.Any(d => d.DeviceId == deviceId);
        }
    }
}