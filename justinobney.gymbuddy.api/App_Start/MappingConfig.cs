using System;
using System.Linq;
using System.Reflection;
using AutoMapper;
using justinobney.gymbuddy.api.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using WebGrease.Css.Extensions;

namespace justinobney.gymbuddy.api
{
    public class StaticConfig
    {
        public static JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

    }

    public class MappingConfig
    {
        public static IMapper Register()
        {
            Config = new MapperConfiguration(cfg =>
            {
                Assembly
                    .GetAssembly(typeof (IAutoMapperConfiguration))
                    .GetExportedTypes()
                    .Where(t => t.IsAbstract == false && typeof (IAutoMapperConfiguration).IsAssignableFrom(t))
                    .ForEach(t =>
                    {
                        var mapping = (IAutoMapperConfiguration) Activator.CreateInstance(t);
                        mapping.Configure(cfg);
                    });
            });

            Instance = Config.CreateMapper();

            return Instance;
        }

        public static MapperConfiguration Config { get; set; }

        public static IMapper Instance { get; set; }
    }
}