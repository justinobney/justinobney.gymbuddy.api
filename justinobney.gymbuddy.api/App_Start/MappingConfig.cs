using AutoMapper;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Requests.Users;

namespace justinobney.gymbuddy.api
{
    public class MappingConfig
    {
        public static IMapper Register()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CreateUserCommand, User>();
                cfg.CreateMap<UpdateUserCommand, User>()
                .ForMember(dest => dest.CreatedAt, opts => opts.Ignore());
            });

            Instance = config.CreateMapper();

            return Instance;
        }

        public static IMapper Instance { get; set; }
    }
}