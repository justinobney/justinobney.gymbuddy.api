using System.Data.Entity;
using System.Linq;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Enums;
using justinobney.gymbuddy.api.Requests.Decorators;
using MediatR;

namespace justinobney.gymbuddy.api.Requests.Friendships
{
    public class GetAllFriendshipsQuery : IRequest<IQueryable<Friendship>>
    {
        public long UserId { get; set; }
    }

    [DoNotValidate]
    [DoNotCommit]
    public class GetAllFriendshipsQueryHandler : IRequestHandler<GetAllFriendshipsQuery, IQueryable<Friendship>>
    {
        private readonly IDbSet<Friendship> _friendships;

        public GetAllFriendshipsQueryHandler(IDbSet<Friendship> friendships)
        {
            _friendships = friendships;
        }


        public IQueryable<Friendship> Handle(GetAllFriendshipsQuery message)
        {
            return _friendships.Where(
                x =>
                    x.UserId == message.UserId
                    && x.Status == FriendshipStatus.Active
                );
        }
    }
}