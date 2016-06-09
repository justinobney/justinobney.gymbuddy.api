using System.Data.Entity;
using System.Linq;
using justinobney.gymbuddy.api.Data.Users;
using justinobney.gymbuddy.api.Helpers;
using justinobney.gymbuddy.api.Requests.Decorators;
using MediatR;

namespace justinobney.gymbuddy.api.Requests.Users
{
    public class UpdateUserProfileImageCommand : IRequest<User>
    {
        public long UserId { get; set; }
        public string ImageDataUri { get; set; }
    }

    [DoNotValidate]
    public class UpdateUserProfileImageCommandHandler : IRequestHandler<UpdateUserProfileImageCommand, User>
    {
        private readonly IDbSet<User> _users;
        private readonly ImageUploader _uploader;

        public UpdateUserProfileImageCommandHandler(IDbSet<User> users, ImageUploader uploader)
        {
            _users = users;
            _uploader = uploader;
        }

        public User Handle(UpdateUserProfileImageCommand message)
        {
            //TODO: move to background task
            var user = _users.First(x => x.Id == message.UserId);
            var imageUrl = _uploader.UploadFromDataUri(message.ImageDataUri);
            user.ProfilePictureUrl = imageUrl;

            return user;
        }
    }
}