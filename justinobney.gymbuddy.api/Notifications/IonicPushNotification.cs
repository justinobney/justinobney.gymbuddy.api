using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using justinobney.gymbuddy.api.Helpers;
using justinobney.gymbuddy.api.Interfaces;
using RestSharp;
using RestSharp.Authenticators;

namespace justinobney.gymbuddy.api.Notifications
{
    public class IonicPushNotification
    {

        public IonicPushNotification(INotificationPayload notification)
        {
            Notification = notification;
        }

        public List<string> Tokens { get; set; }
        public bool Production { get; set; }
        public INotificationPayload Notification { get; set; }

        public IRestResponse Send(IRestClient client)
        {
            if (!Tokens.Any())
            {
                return new RestResponse {StatusCode = HttpStatusCode.BadRequest, Content = "No Tokens Provided"};
            }

            var ionicRequest = new RestRequest("/push", Method.POST);
            ionicRequest.AddHeader("X-Ionic-Application-Id", ConfigurationManager.AppSettings["Ionic-Application-Id"]);
            ionicRequest.AddHeader("Content-Type", "application/json");
            ionicRequest.JsonSerializer = new CamelCaseSerializer();
            ionicRequest.AddJsonBody(this);

            client.BaseUrl = new Uri("https://push.ionic.io/api/v1");
            client.Authenticator = new HttpBasicAuthenticator(ConfigurationManager.AppSettings["Ionic-Api-Key"], "");
            var response = client.Execute(ionicRequest);
            Debug.WriteLine(response.Content);
            return response;
        }
    }

    // Used to be able to properly deserialize into a typed NotificationPayload from JSON when known
    public class IonicPushNotification<T> : IonicPushNotification
    {
        public IonicPushNotification(NotificationPayload<T> notification) : base(notification)
        {
        }
    }
}