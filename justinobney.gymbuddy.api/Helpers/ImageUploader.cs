using System;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using justinobney.gymbuddy.api.Interfaces;

namespace justinobney.gymbuddy.api.Helpers
{
    public class ImageUploader : IImageUploader
    {
        private readonly Cloudinary _cloudinary;

        public ImageUploader(Cloudinary cloudinary)
        {
            _cloudinary = cloudinary;
        }

        /// <summary>
        /// Uploads a Base64 encoded image to a 3rd party service
        /// </summary>
        /// <param name="dataUri"></param>
        /// <returns>Public Url of the uploaded image</returns>
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