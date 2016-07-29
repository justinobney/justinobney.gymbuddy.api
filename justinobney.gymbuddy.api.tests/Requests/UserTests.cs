using System;
using System.Data.Entity;
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
        private UserTestArrangement Arrange()
        {
            var arrangement = new UserTestArrangement
            {
                Context = Context.GetInstance<AppContext>(),
                Devices = Context.GetSet<Device>(),
                Users = Context.GetSet<User>()
            };

            var createAndModifiedDate = Convert.ToDateTime("01/07/2016");
            arrangement.Users.Add(new User { Id = 7755, CreatedAt = createAndModifiedDate, ModifiedAt = createAndModifiedDate, FacebookUserId = "fbid1" });
            arrangement.Users.Add(new User { Id = 192837, CreatedAt = createAndModifiedDate, ModifiedAt = createAndModifiedDate, FacebookUserId = "fbid2", ProfilePictureUrl = "something" });
            arrangement.Devices.Add(new Device { DeviceId = "123654", UserId = 7755, ClientVersion = "v1.0.0.1", CreatedAt = createAndModifiedDate, ModifiedAt = createAndModifiedDate });
            arrangement.Devices.Add(new Device { DeviceId = "75555666", UserId = 192837, ClientVersion = "v1.0.0.1", CreatedAt = createAndModifiedDate, ModifiedAt = createAndModifiedDate });
            return arrangement;
        }

        [Test]
        public void UpdateDeviceCommand_SetsDevicePushToken()
        {
            var context = Context.GetInstance<AppContext>();
            var devices = Context.GetSet<Device>();
            var users = Context.GetSet<User>();
            users.Add(new User { Id = 123 });
            devices.Add(new Device { DeviceId = "12345", UserId = 123 });
            Mediator.Send(new UpdateDeviceCommand
            {
                DeviceId = "12345",
                PushToken = "foo-bar",
                Platform = "iOS",
                FacebookUserId = "FACE123"
            });

            devices.First(d => d.DeviceId == "12345").PushToken.ShouldBe("foo-bar");
            devices.First(d => d.DeviceId == "12345").Platform.ShouldBe("iOS");
            users.First(d => d.Id == 123).FacebookUserId.ShouldBe("FACE123");
            context.Received(1).SaveChanges();
        }
        
        [Test]
        public void UpdateDeviceCommand_UpdatesDeviceModifiedAtProperty()
        {
            var arrangement = Arrange();

            Mediator.Send(new UpdateDeviceCommand { DeviceId = "123654", PushToken = "foo-bar", Platform = "web", FacebookUserId = "112233", ClientVersion = null });

            var responseToAssert = arrangement.Devices.FirstOrDefault(d => d.DeviceId == "123654");
            if (responseToAssert?.ModifiedAt != null)
                responseToAssert?.ModifiedAt.Value.Date.ShouldNotBe(Convert.ToDateTime("01/07/2016").Date);
            arrangement.Context.Received(1).SaveChanges();
        }

        [Test]
        public void UpdateDeviceCommand_NullDoesNotUpdateDeviceClientVersion()
        {
            var arrangement = Arrange();

            Mediator.Send(new UpdateDeviceCommand { DeviceId = "123654", PushToken = "foo-bar", Platform = "web", FacebookUserId = "112233", ClientVersion = null });

            arrangement.Devices.First(d => d.DeviceId == "123654").ClientVersion.ShouldBe("v1.0.0.1");
            arrangement.Context.Received(1).SaveChanges();
        }

        [Test]
        public void UpdateDeviceCommand_ClientVersionEmptyStringDoesNotUpdateDeviceClientVersion()
        {
            var arrangement = Arrange();

            Mediator.Send(new UpdateDeviceCommand { DeviceId = "123654", PushToken = "foo-bar", Platform = "web", FacebookUserId = "112233", ClientVersion = string.Empty });

            arrangement.Devices.First(d => d.DeviceId == "123654").ClientVersion.ShouldBe("v1.0.0.1");
            arrangement.Context.Received(1).SaveChanges();
        }

        [Test]
        public void UpdateDeviceCommand_SetsDeviceClientVersion()
        {
            var arrangement = Arrange();

            Mediator.Send(new UpdateDeviceCommand { DeviceId = "123654", PushToken = "foo-bar", Platform = "web", FacebookUserId = "112233", ClientVersion = "v1.0.0.3" });

            arrangement.Devices.First(d => d.DeviceId == "123654").ClientVersion.ShouldBe("v1.0.0.3");
            arrangement.Context.Received(1).SaveChanges();
        }

        [Test]
        public void UpdateDeviceCommand_ClientVersionSpacedEmptyStringDoesNotUpdateDeviceClientVersion()
        {
            var arrangement = Arrange();

            Mediator.Send(new UpdateDeviceCommand { DeviceId = "123654", PushToken = "foo-bar", Platform = "web", FacebookUserId = "112233", ClientVersion = "    " });

            arrangement.Devices.First(d => d.DeviceId == "123654").ClientVersion.ShouldBe("v1.0.0.1");
            arrangement.Context.Received(1).SaveChanges();
        }

        [Test]
        public void UpdateDeviceCommand_DoesNotUpdateUserWithoutFacebookId()
        {
            var arrangement = Arrange();

            Mediator.Send(new UpdateDeviceCommand { DeviceId = "123654", PushToken = "foo-bar", Platform = "web", ClientVersion = null });

            arrangement.Users.FirstOrDefault(d => d.Id == 7755).ProfilePictureUrl.ShouldBeNull();
            arrangement.Users.FirstOrDefault(d => d.Id == 7755).FacebookUserId.ShouldBe("fbid1");
            arrangement.Users.FirstOrDefault(d => d.Id == 7755).ModifiedAt.Value.Date.ShouldNotBe(DateTime.Now.Date);
            arrangement.Context.Received(1).SaveChanges();
        }

        [Test]
        public void UpdateDeviceCommand_UpdatesUserFacebookId()
        {
            var arrangement = Arrange();

            Mediator.Send(new UpdateDeviceCommand { DeviceId = "123654", PushToken = "foo-bar", Platform = "web", FacebookUserId = "wootwoot", ClientVersion = null });

            arrangement.Users.FirstOrDefault(d => d.Id == 7755).FacebookUserId.ShouldBe("wootwoot");
            arrangement.Context.Received(1).SaveChanges();
        }

        [Test]
        public void UpdateDeviceCommand_DoesNotUpdateUserFacebookIdIfSame()
        {
            var arrangement = Arrange();

            Mediator.Send(new UpdateDeviceCommand { DeviceId = "75555666", PushToken = "foo-bar", Platform = "web", FacebookUserId = "fbid2", ClientVersion = null });

            arrangement.Users.FirstOrDefault(d => d.Id == 192837).FacebookUserId.ShouldBe("fbid2");
            arrangement.Context.Received(1).SaveChanges();
        }

        [Test]
        public void UpdateDeviceCommand_UpdatesUserProfilePicture()
        {
            var arrangement = Arrange();

            Mediator.Send(new UpdateDeviceCommand { DeviceId = "123654", PushToken = "foo-bar", Platform = "web", FacebookUserId="fbid1", ClientVersion = null });

            arrangement.Users.FirstOrDefault(d => d.Id == 7755).ProfilePictureUrl.ShouldNotBeNull();
            arrangement.Users.FirstOrDefault(d => d.Id == 7755).ProfilePictureUrl.ShouldNotBeEmpty();

            arrangement.Context.Received(1).SaveChanges();
        }

        [Test]
        public void UpdateDeviceCommand_DoesNotUpdateUserProfilePictureIfOneExists()
        {
            var arrangement = Arrange();

            Mediator.Send(new UpdateDeviceCommand { DeviceId = "75555666", PushToken = "foo-bar", Platform = "web", FacebookUserId = "fbid2", ClientVersion = "191.191.111" });

            arrangement.Users.FirstOrDefault(d => d.Id == 192837).ProfilePictureUrl.ShouldBe("something");
            arrangement.Context.Received(1).SaveChanges();
        }

        [Test]
        public void UpdateDeviceCommand_UpdatesUserModifiedAtProperty()
        {
            var arrangement = Arrange();

            Mediator.Send(new UpdateDeviceCommand { DeviceId = "123654", PushToken = "foo-bar", Platform = "web", FacebookUserId = "112233", ClientVersion = null });
            Mediator.Send(new UpdateDeviceCommand { DeviceId = "75555666", PushToken = "foo-bar", Platform = "web", FacebookUserId = "33222", ClientVersion = null });

            arrangement.Users.FirstOrDefault(d => d.Id == 7755).ModifiedAt.Value.Date.ShouldBe(DateTime.Now.Date);
            arrangement.Users.FirstOrDefault(d => d.Id == 192837).ModifiedAt.Value.Date.ShouldBe(DateTime.Now.Date);
            arrangement.Context.Received(2).SaveChanges();
        }

        [Test]
        public void UpdateDeviceCommand_DoesNotUpdateUserModifiedAtPropertyIfNothingChanged()
        {
            var arrangement = Arrange();
            
            Mediator.Send(new UpdateDeviceCommand { DeviceId = "75555666", PushToken = "foo-bar", Platform = "web", FacebookUserId = "fbid2", ClientVersion = "v293.4" });

            arrangement.Users.FirstOrDefault(d => d.Id == 192837).ModifiedAt.Value.Date.ShouldNotBe(DateTime.Now.Date);
            arrangement.Context.Received(1).SaveChanges();
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

        [Test]
        public void UpdateNotificationSettingsCommandReturnsNullOnInvalidUserId()
        {
            var command = new UpdateNotificationSettingsCommand
            {
                UserId = 1,
                SilenceAllNotifications = true
            };

            var response = Mediator.Send(command);
            response.ShouldBe(null);
        }
    }

    public class UserTestArrangement
    {
        public AppContext Context { get; set; }
        public IDbSet<Device> Devices { get; set; }
        public IDbSet<User> Users { get; set; }
    }
}