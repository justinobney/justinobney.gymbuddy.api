using System;
using System.IO;
using System.Web.Http;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using MediatR;

namespace justinobney.gymbuddy.api.Controllers
{
    public class MediaController : AuthenticatedController
    {
        public MediaController(IMediator mediator) : base(mediator)
        {
        }
        
        public IHttpActionResult PostMedia(ImagePayload payload)
        {
            byte[] imageBytes = Convert.FromBase64String(payload.Content);
            MemoryStream ms = new MemoryStream(imageBytes, 0,
              imageBytes.Length);

            ms.Write(imageBytes, 0, imageBytes.Length);
            ms.Position =0;

            var account = new Account(
                "justin-obney",
                "241728748886935",
                "DuOiT0rBvBZaaeyrDnpmaVEJRiY");

            var cloudinary = new Cloudinary(account);

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription($"{Guid.NewGuid()}", ms)
            };
            var uploadResult = cloudinary.Upload(uploadParams);
            
            return Ok(uploadResult);
        }
    }
    
    public class ImagePayload
    {
        public string Content { get; set; }
    }
}