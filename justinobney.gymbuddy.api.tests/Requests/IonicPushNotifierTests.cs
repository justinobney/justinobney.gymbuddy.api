using System.Collections.Generic;
using justinobney.gymbuddy.api.Requests.External;
using justinobney.gymbuddy.api.tests.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;

namespace justinobney.gymbuddy.api.tests.Requests
{
    [TestFixture]
    public class IonicPushNotifierTests : BaseTest
    {
        [Test]
        public void NotifierShouldSerializeCorrectly()
        {
            var payload = new FooPayload {Foo = "Bar"};
            var notification = new NotificationPayload<FooPayload>(payload)
            {
                Alert = "Alert",
                Title = "Title"
            };

            var notifier = new IonicPushNotification(notification)
            {
                Tokens = new List<string> { "123" }
            };

            var serializationSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            var expected = "{\"tokens\":[\"123\"],\"production\":false,\"notification\":{\"alert\":\"Alert\",\"title\":\"Title\",\"android\":{\"payload\":{\"foo\":\"Bar\"}},\"ios\":{\"payload\":{\"foo\":\"Bar\"}}}}";
            JsonConvert.SerializeObject(notifier, serializationSettings).ShouldBe(expected);
        }

        public class FooPayload
        {
            public string Foo { get; set; }
        }
    }
}