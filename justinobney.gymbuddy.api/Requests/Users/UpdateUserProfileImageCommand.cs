using System;
using System.Data.Entity;
using System.Linq;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
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
        private readonly Cloudinary _cloudinary;

        public UpdateUserProfileImageCommandHandler(IDbSet<User> users, Cloudinary cloudinary)
        {
            _users = users;
            _cloudinary = cloudinary;
        }

        public User Handle(UpdateUserProfileImageCommand message)
        {
            var user = _users.First(x => x.Id == message.UserId);
            var ms = ImageHelpers.ConvertDataUriToMemoryStream(message.ImageDataUri);
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription($"{Guid.NewGuid()}", ms)
            };

            var uploadResult = _cloudinary.Upload(uploadParams);
            user.ProfilePictureUrl = uploadResult.Uri.ToString();

            return user;
        }
    }
}