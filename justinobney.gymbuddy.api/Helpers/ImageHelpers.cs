using System;
using System.IO;
using System.Linq;

namespace justinobney.gymbuddy.api.Helpers
{
    public static class ImageHelpers
    {
        public static MemoryStream ConvertDataUriToMemoryStream(string dataUri)
        {
            var data = dataUri.Split(',').Last();
            var imageBytes = Convert.FromBase64String(data);
            var ms = new MemoryStream(imageBytes, 0,
                imageBytes.Length);

            ms.Write(imageBytes, 0, imageBytes.Length);
            ms.Position = 0;
            return ms;
        }
    }
}