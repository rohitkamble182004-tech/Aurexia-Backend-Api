namespace Fashion.Api.Core.Interfaces
{
    public interface IImageService
    {
        string UploadImage(byte[] imageBytes, string fileName);
        Task<string> UploadProductImage(IFormFile file);
    }
}
