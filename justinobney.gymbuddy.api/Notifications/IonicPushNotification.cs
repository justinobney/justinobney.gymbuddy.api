using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using justinobney.gymbuddy.api.Helpers;
using RestSharp;
using RestSharp.Authenticators;

namespace justinobney.gymbuddy.api.Notifications
{
    public class IonicPushNotification
    {

        public IonicPushNotification(NotificationPayload notification)
        {
            Notification = notification;
        }

        public List<string> Tokens { get; set; }
        public bool Production { get; set; }
        public NotificationPayload Notification { get; set; }

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
            return response;
        }
    }
}