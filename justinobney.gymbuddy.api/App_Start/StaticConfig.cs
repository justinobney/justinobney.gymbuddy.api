using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace justinobney.gymbuddy.api
{
    public class StaticConfig
    {
        public static JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

    }
}