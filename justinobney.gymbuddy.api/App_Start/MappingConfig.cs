using System;
using System.Linq;
using System.Reflection;
using AutoMapper;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Interfaces;
using justinobney.gymbuddy.api.Requests.Users;
using WebGrease.Css.Extensions;

namespace justinobney.gymbuddy.api
{
    public class MappingConfig
    {
        public static IMapper Register()
        {
            var config = new MapperConfiguration(cfg =>
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

            Instance = config.CreateMapper();

            return Instance;
        }

        public static IMapper Instance { get; set; }
    }
}