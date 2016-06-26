using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using justinobney.gymbuddy.api.Helpers;
using RestSharp;

namespace justinobney.gymbuddy.api.Notifications
{
    public class IonicPushNotification
    {

        public IonicPushNotification(NotificationPayload notification)
        {
            Notification = notification;
        }

        public List<string> Tokens { get; set; }
        public string Profile { get; set; } = "letmelift__prod";
        public bool Production { get; set; }

        public string Message { get; set; }
        public string Title { get; set; }

        public NotificationPayload Notification { get; set; }

        public IRestResponse Send(IRestClient client)
        {
            if (!Tokens.Any())
            {
                return new RestResponse {StatusCode = HttpStatusCode.BadRequest, Content = "No Tokens Provided"};
            }

            var ionicRequest = new RestRequest("/push/notifications", Method.POST);
//            ionicRequest.AddHeader("X-Ionic-Application-Id", ConfigurationManager.AppSettings["Ionic-Application-Id"]);
            ionicRequest.AddHeader("Content-Type", "application/json");
            ionicRequest.AddHeader("Authorization", ConfigurationManager.AppSettings["Ionic-Token"]);
            ionicRequest.JsonSerializer = new CamelCaseSerializer();
            ionicRequest.AddJsonBody(this);

            client.BaseUrl = new Uri("https://api.ionic.io");
//            client.Authenticator = new HttpBasicAuthenticator(ConfigurationManager.AppSettings["Ionic-Api-Key"], "");
            var response = client.Execute(ionicRequest);
            return response;
        }
    }
}