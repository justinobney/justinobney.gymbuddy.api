using System;
using System.Linq;
using justinobney.gymbuddy.api.Data;
using justinobney.gymbuddy.api.Data.Devices;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Requests.Users;
using justinobney.gymbuddy.api.tests.Helpers;
using NSubstitute;
using NUnit.Framework;

namespace justinobney.gymbuddy.api.tests.Requests
{
    [TestFixture]
    public class UserTests : BaseTest
    {
        [Test]
        public void UpdateDeviceCommand_SetsDevicePushToken()
        {
            var context = Context.GetInstance<AppContext>();
            var devices = Context.GetSet<Device>();
            var users = Context.GetSet<User>();
            users.Add(new User {Id = 123 });
            devices.Add(new Device {DeviceId = "12345", UserId = 123});
            Mediator.Send(new UpdateDeviceCommand
            {
                DeviceId = "12345",
                PushToken = "foo-bar",
                Platform = "iOS",
                FacebookUserId = "FACE123"
            });

            devices.First(d=>d.DeviceId == "12345").PushToken.ShouldBe("foo-bar");
            devices.First(d=>d.DeviceId == "12345").Platform.ShouldBe("iOS");
            users.First(d=>d.Id == 123).FacebookUserId.ShouldBe("FACE123");
            context.Received(1).SaveChanges();
        }

        [Test]
        public void CreateUserCommandEnablesGymWorkoutNotifications()
        {
            var command = new CreateUserCommand
            {
                Id = 1,
                Name = "MONSTAR",
                DeviceId = "123",
                FilterFitnessLevel = Enums.FitnessLevel.Beginner,
                FacebookUserId = "1122334455",
                FilterGender = Enums.Gender.Male,
                Gender = Enums.Gender.Male,
                FitnessLevel = Enums.FitnessLevel.Beginner,
                Platform = "web"
            };

            var response = Mediator.Send(command);
            response.NewGymWorkoutNotifications.ShouldBe(true);
        }

        [Test]
        public void CreateUserCommandEnablesSquadWorkoutNotifications()
        {
            var command = new CreateUserCommand
            {
                Id = 1,
                Name = "MONSTAR",
                DeviceId = "123",
                FilterFitnessLevel = Enums.FitnessLevel.Beginner,
                FacebookUserId = "1122334455",
                FilterGender = Enums.Gender.Male,
                Gender = Enums.Gender.Male,
                FitnessLevel = Enums.FitnessLevel.Beginner,
                Platform = "web"
            };

            var response = Mediator.Send(command);
            response.NewSquadWorkoutNotifications.ShouldBe(true);
        }

        [Test]
        public void CreateUserCommandSilenceNotificationsDisabled()
        {
            var command = new CreateUserCommand
            {
                Id = 1,
                Name = "MONSTAR",
                DeviceId = "123",
                FilterFitnessLevel = Enums.FitnessLevel.Beginner,
                FacebookUserId = "1122334455",
                FilterGender = Enums.Gender.Male,
                Gender = Enums.Gender.Male,
                FitnessLevel = Enums.FitnessLevel.Beginner,
                Platform = "web"
            };

            var response = Mediator.Send(command);
            response.SilenceAllNotifications.ShouldBe(false);
        }

        [Test]
        public void UpdateNotificationSettingsCommandUpdatesGymWorkoutNotifications()
        {
            var users = Context.GetSet<User>();

            var user = new User
            {
                Id = 1,
                Name = "MONSTAR",
                NewGymWorkoutNotifications = false
            };
            users.Attach(user);

            var trueCommandTest = new UpdateNotificationSettingsCommand
            {
                UserId = 1,
                NewGymWorkoutNotifications = true
            };

            var trueTestResponse = Mediator.Send(trueCommandTest);
            trueTestResponse.NewGymWorkoutNotifications.ShouldBe(true);


            var falseCommandTest = new UpdateNotificationSettingsCommand
            {
                UserId = 1,
                NewGymWorkoutNotifications = false
            };

            var falseTestResponse = Mediator.Send(falseCommandTest);
            falseTestResponse.NewGymWorkoutNotifications.ShouldBe(false);
        }

        [Test]
        public void UpdateNotificationSettingsCommandUpdatesSquadWorkoutNotifications()
        {
            var users = Context.GetSet<User>();

            var user = new User
            {
                Id = 1,
                Name = "MONSTAR",
                NewSquadWorkoutNotifications = false
            };
            users.Attach(user);

            var trueCommandTest = new UpdateNotificationSettingsCommand
            {
                UserId = 1,
                NewSquadWorkoutNotifications = true
            };

            var trueTestResponse = Mediator.Send(trueCommandTest);
            trueTestResponse.NewSquadWorkoutNotifications.ShouldBe(true);


            var falseCommandTest = new UpdateNotificationSettingsCommand
            {
                UserId = 1,
                NewSquadWorkoutNotifications = false
            };

            var falseTestResponse = Mediator.Send(falseCommandTest);
            falseTestResponse.NewSquadWorkoutNotifications.ShouldBe(false);
        }

        [Test]
        public void UpdateNotificationSettingsCommandUpdatesSilenceAllNotifications()
        {
            var users = Context.GetSet<User>();

            var user = new User
            {
                Id = 1,
                Name = "MONSTAR",
                NewSquadWorkoutNotifications = true,
                NewGymWorkoutNotifications = true,
                SilenceAllNotifications = false
            };
            users.Attach(user);

            var trueCommandTest = new UpdateNotificationSettingsCommand
            {
                UserId = 1,
                SilenceAllNotifications = true
            };

            var trueTestResponse = Mediator.Send(trueCommandTest);
            trueTestResponse.SilenceAllNotifications.ShouldBe(true);


            var falseCommandTest = new UpdateNotificationSettingsCommand
            {
                UserId = 1,
                SilenceAllNotifications = false
            };

            var falseTestResponse = Mediator.Send(falseCommandTest);
            falseTestResponse.SilenceAllNotifications.ShouldBe(false);
        }

        [Test]
        public void UpdateNotificationSettingsCommandUpdatesModifiedAtTime()
        {
            var users = Context.GetSet<User>();

            var user = new User
            {
                Id = 1,
                Name = "MONSTAR",
                NewSquadWorkoutNotifications = true,
                NewGymWorkoutNotifications = true,
                SilenceAllNotifications = false,
                ModifiedAt = Convert.ToDateTime("01-01-2016")
            };
            users.Attach(user);

            var command = new UpdateNotificationSettingsCommand
            {
                UserId = 1,
                SilenceAllNotifications = true
            };

            var response = Mediator.Send(command);
            response.ModifiedAt.ShouldNotBe(Convert.ToDateTime("01-01-2016"));
            response.ModifiedAt.ShouldNotBe(null);
            response.ModifiedAt.HasValue.ShouldBe(true);
            response.ModifiedAt.Value.Date.ShouldBe(DateTime.UtcNow.Date);
        }

    }
}