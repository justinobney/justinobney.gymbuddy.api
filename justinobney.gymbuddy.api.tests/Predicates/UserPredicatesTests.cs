using System.Linq;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Enums;
using justinobney.gymbuddy.api.tests.Helpers;
using NUnit.Framework;

namespace justinobney.gymbuddy.api.tests.Predicates
{
    [TestFixture]
    public class UserPredicatesTests : BaseTest
    {
        [Test]
        public void RestrictMemberByGender_ProperlyRestrictsGenders()
        {
            var users = Context.GetSet<User>();
            users.Attach(new User {Gender = Gender.Male, FilterGender = GenderFilter.Male});
            users.Attach(new User {Gender = Gender.Male, FilterGender = GenderFilter.Female});
            users.Attach(new User {Gender = Gender.Male, FilterGender = GenderFilter.Both});

            users.Attach(new User { Gender = Gender.Female, FilterGender = GenderFilter.Male });
            users.Attach(new User { Gender = Gender.Female, FilterGender = GenderFilter.Female });
            users.Attach(new User { Gender = Gender.Female, FilterGender = GenderFilter.Both });

            var ladyForLady = new User {Gender = Gender.Female, FilterGender = GenderFilter.Female};
            var ladyForMen = new User {Gender = Gender.Female, FilterGender = GenderFilter.Male};
            var ladyForAny = new User {Gender = Gender.Female, FilterGender = GenderFilter.Both};

            var manForLady = new User { Gender = Gender.Male, FilterGender = GenderFilter.Female };
            var manForMen = new User { Gender = Gender.Male, FilterGender = GenderFilter.Male };
            var manForAny = new User { Gender = Gender.Male, FilterGender = GenderFilter.Both };

            users.Count(UserPredicates.RestrictMemberByGender(ladyForLady)).ShouldBe(2);
            users.Count(UserPredicates.RestrictMemberByGender(ladyForMen)).ShouldBe(2);
            users.Count(UserPredicates.RestrictMemberByGender(ladyForAny)).ShouldBe(4);

            users.Count(UserPredicates.RestrictMemberByGender(manForLady)).ShouldBe(2);
            users.Count(UserPredicates.RestrictMemberByGender(manForMen)).ShouldBe(2);
            users.Count(UserPredicates.RestrictMemberByGender(manForAny)).ShouldBe(4);
        }

        [Test]
        public void RestrictMemberByFitnessLevel_ProperlyRestrictsWeaklings()
        {
            var users = Context.GetSet<User>();
            users.Attach(new User { FitnessLevel = FitnessLevel.Beginner, FilterFitnessLevel = FitnessLevel.Beginner});
            users.Attach(new User { FitnessLevel = FitnessLevel.Beginner, FilterFitnessLevel = FitnessLevel.Intermediate});
            users.Attach(new User { FitnessLevel = FitnessLevel.Beginner, FilterFitnessLevel = FitnessLevel.Advanced});
            users.Attach(new User { FitnessLevel = FitnessLevel.Intermediate, FilterFitnessLevel = FitnessLevel.Beginner });
            users.Attach(new User { FitnessLevel = FitnessLevel.Intermediate, FilterFitnessLevel = FitnessLevel.Intermediate });
            users.Attach(new User { FitnessLevel = FitnessLevel.Intermediate, FilterFitnessLevel = FitnessLevel.Advanced });
            users.Attach(new User { FitnessLevel = FitnessLevel.Advanced, FilterFitnessLevel = FitnessLevel.Beginner });
            users.Attach(new User { FitnessLevel = FitnessLevel.Advanced, FilterFitnessLevel = FitnessLevel.Intermediate });
            users.Attach(new User { FitnessLevel = FitnessLevel.Advanced, FilterFitnessLevel = FitnessLevel.Advanced });

            var helpMeAnyone = new User { FitnessLevel = FitnessLevel.Beginner, FilterFitnessLevel = FitnessLevel.Beginner };
            var helpMeStrongPeople = new User { FitnessLevel = FitnessLevel.Beginner, FilterFitnessLevel = FitnessLevel.Advanced };
            var strongPeopleOnly = new User { FitnessLevel = FitnessLevel.Advanced, FilterFitnessLevel = FitnessLevel.Advanced };
            var noNoobs = new User { FitnessLevel = FitnessLevel.Intermediate, FilterFitnessLevel = FitnessLevel.Intermediate };


            users.Count(UserPredicates.RestrictMemberByFitnessLevel(helpMeAnyone)).ShouldBe(3);
            users.Count(UserPredicates.RestrictMemberByFitnessLevel(helpMeStrongPeople)).ShouldBe(1);
            users.Count(UserPredicates.RestrictMemberByFitnessLevel(strongPeopleOnly)).ShouldBe(3);
            users.Count(UserPredicates.RestrictMemberByFitnessLevel(noNoobs)).ShouldBe(4);
        }
    }
}