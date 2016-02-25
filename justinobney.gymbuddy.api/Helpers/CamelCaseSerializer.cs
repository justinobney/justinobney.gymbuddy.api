using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp.Serializers;

namespace justinobney.gymbuddy.api.Helpers
{
    public class CamelCaseSerializer : ISerializer
    {
        public string Serialize(object obj)
        {
            var serializationSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            return JsonConvert.SerializeObject(obj, serializationSettings);
        }

        public string RootElement { get; set; }
        public string Namespace { get; set; }
        public string DateFormat { get; set; }
        public string ContentType { get; set; } = "application/json";
    }
}