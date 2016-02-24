using System;
using System.Collections.Generic;
using System.Configuration;
using RestSharp;
using RestSharp.Authenticators;

namespace justinobney.gymbuddy.api.Requests.External
{
    public class IonicPushNotification<T>
    {

        public IonicPushNotification(NotificationPayload<T> notification)
        {
            Notification = notification;
        }

        public List<string> Tokens { get; set; }
        public bool Production { get; set; }
        public NotificationPayload<T> Notification { get; set; }

        public void Send(IRestClient client)
        {
            var ionicRequest = new RestRequest("/push", Method.POST);
            ionicRequest.AddHeader("X-Ionic-Application-Id", ConfigurationManager.AppSettings["Ionic-Application-Id"]);
            ionicRequest.AddHeader("Content-Type", "application/json");
            ionicRequest.JsonSerializer = new CamelCaseSerializer();
            ionicRequest.AddJsonBody(this);

            client.BaseUrl = new Uri("https://push.ionic.io/api/v1");
            client.Authenticator = new HttpBasicAuthenticator(ConfigurationManager.AppSettings["Ionic-Api-Key"], "");
            client.Execute(ionicRequest);
        }
    }
    
    public class NotificationPayload<T>
    {
        public NotificationPayload(T payload)
        {
            Android = new Android<T>
            {
                Payload = payload
            };

            Ios = new Ios<T>
            {
                Payload = payload
            };
        }
        public string Alert { get; set; }
        public string Title { get; set; }
        public Android<T> Android { get; set; }
        public Ios<T> Ios { get; set; }
    }
    
    public class Android<T>
    {
        public T Payload { get; set; }
    }
    
    public class Ios<T>
    {
        public T Payload { get; set; }
        public int? Badge { get; set; }
    }
}