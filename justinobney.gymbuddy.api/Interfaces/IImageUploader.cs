namespace justinobney.gymbuddy.api.Interfaces
{
    public interface IImageUploader
    {
        /// <summary>
        /// Uploads a Base64 encoded image to a 3rd party service
        /// </summary>
        /// <param name="dataUri"></param>
        /// <returns>Public Url of the uploaded image</returns>
        string UploadFromDataUri(string dataUri);
    }
}