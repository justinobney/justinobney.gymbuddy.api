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
    }
}