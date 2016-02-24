using System.Collections.Generic;
using System.Diagnostics;
using justinobney.gymbuddy.api.Data.Appointments;
using justinobney.gymbuddy.api.Interfaces;
using justinobney.gymbuddy.api.Requests.Appointments;
using justinobney.gymbuddy.api.Requests.External;
using justinobney.gymbuddy.api.tests.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NSubstitute;
using NUnit.Framework;
using RestSharp;

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

            var notifier = new IonicPushNotification<FooPayload>(notification)
            {
                Tokens = new List<string> { "123" }
            };

            var serializationSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            var expected = "{\"tokens\":[\"123\"],\"production\":false,\"notification\":{\"alert\":\"Alert\",\"title\":\"Title\",\"android\":{\"payload\":{\"foo\":\"Bar\"}},\"ios\":{\"payload\":{\"foo\":\"Bar\"},\"badge\":null}}}";
            JsonConvert.SerializeObject(notifier, serializationSettings).ShouldBe(expected);

            var expected2 = "{\"tokens\":null,\"production\":false,\"notification\":{\"alert\":null,\"title\":null,\"android\":{\"payload\":null},\"ios\":{\"payload\":null,\"badge\":null}}}";
            JsonConvert.SerializeObject(new IonicPushNotification<object>(new NotificationPayload<object>(null)), serializationSettings).ShouldBe(expected2);
        }

        [Test]
        public void CreateAppointmentNotifier_CallsRestSharpMethod()
        {
            var restClient = Substitute.For<RestClient>();
            Context.Container.Configure(container => container.For<IRestClient>().Use(restClient));
            Context.Register<IPostRequestHandler<CreateAppointmentCommand, Appointment>, CreateAppointmentNotifier>();
            var handler = Context.GetInstance<IPostRequestHandler<CreateAppointmentCommand, Appointment>>();

            var request = new CreateAppointmentCommand {UserId = 1};
            var response = new Appointment {UserId = 1};

            IonicPushNotification<object> pushNotification;

            restClient.WhenForAnyArgs(client => client.Post(new RestRequest())).Do(info =>
            {
                var restRequest = info.Arg<RestRequest>();
                var jsonPayload = restRequest.Parameters.Find(p => p.Name == "application/json");
                Debug.WriteLine(jsonPayload.Value);
                pushNotification = JsonConvert.DeserializeObject<IonicPushNotification<object>>((string)jsonPayload.Value);

                restRequest.Resource.ShouldBe("/push");
                restRequest.Parameters.Find(p => p.Name == "X-Ionic-Application-Id").ShouldNotBeNull();
                pushNotification.Notification.Alert.ShouldBe("New Appointment Available");
            });
            
            handler.Notify(request, response);
            restClient.ReceivedWithAnyArgs(1).Post(new RestRequest());

            ConfigIoC();
        }

        public class FooPayload
        {
            public string Foo { get; set; }
        }
    }
}