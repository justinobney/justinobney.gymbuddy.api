using System;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace justinobney.gymbuddy.api.Helpers
{
    public class ImageUploader
    {
        private readonly Cloudinary _cloudinary;

        public ImageUploader(Cloudinary cloudinary)
        {
            _cloudinary = cloudinary;
        }

        public string UploadFromDataUri(string dataUri)
        {
            var ms = ImageHelpers.ConvertDataUriToMemoryStream(dataUri);
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription($"{Guid.NewGuid()}", ms)
            };

            var uploadResult = _cloudinary.Upload(uploadParams);
            return uploadResult.Uri.ToString();
        }
    }
}