using System;
using System.Data.Entity;
using System.Threading.Tasks;
using AutoMapper;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Enums;
using justinobney.gymbuddy.api.Interfaces;
using justinobney.gymbuddy.api.Requests.Decorators;
using MediatR;

namespace justinobney.gymbuddy.api.Requests.Users
{
    public class UpdateUserCommand : IRequest
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public FitnessLevel FitnessLevel { get; set; }
        public FitnessLevel FilterFitnessLevel { get; set; }
        public Gender Gender { get; set; }
        public Gender FilterGender { get; set; }
    }

    [DoNotValidate]
    public class UpdateUserCommandHandler : RequestHandler<UpdateUserCommand>
    {
        private readonly IMapper _mapper;
        private readonly IDbSet<User> _users;

        public UpdateUserCommandHandler(IMapper mapper, IDbSet<User> users)
        {
            _mapper = mapper;
            _users = users;
        }

        protected override void HandleCore(UpdateUserCommand message)
        {
            var user = _users.Find(message.Id);
            _mapper.Map(message, user);
            user.ModifiedAt = DateTime.UtcNow;
        }
    }

    public class UpdateUserCommandMapper : IAutoMapperConfiguration
    {
        public void Configure(IMapperConfiguration cfg)
        {
            cfg.CreateMap<UpdateUserCommand, User>()
                .ForMember(dest => dest.CreatedAt, opts => opts.Ignore());
        }
    }
}