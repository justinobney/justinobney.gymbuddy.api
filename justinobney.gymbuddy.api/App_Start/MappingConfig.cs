using System;
using System.Linq;
using System.Reflection;
using AutoMapper;
using justinobney.gymbuddy.api.Interfaces;
using WebGrease.Css.Extensions;

namespace justinobney.gymbuddy.api
{
    public class MappingConfig
    {
        public static IMapper Register()
        {
            Config = new MapperConfiguration(cfg =>
            {
                Assembly
                    .GetExecutingAssembly()
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