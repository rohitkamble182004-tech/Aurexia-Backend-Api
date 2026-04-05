using Fashion.Api.Core.Interfaces;

namespace Fashion.Api.Infrastructure.Services
{
    public class AzureBlobService : IImageService
    {
        public string UploadImage(byte[] imageBytes, string fileName)
        {
            // Placeholder for Azure Blob logic
            return "https://azure.blob.fake/url";
        }

        public Task<string> UploadProductImage(IFormFile file)
        {
            throw new NotImplementedException();
        }
    }
}