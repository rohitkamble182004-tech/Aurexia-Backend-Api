using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Fashion.Api.Core.Interfaces;
using Microsoft.Extensions.Options;
using Fashion.Api.Infrastructure.Configurations;

namespace Fashion.Api.Infrastructure.Services
{
    public class CloudinaryService : IImageService
    {
        private readonly Cloudinary _cloudinary;


        public CloudinaryService(IOptions<CloudinarySettings> options)
        {
            var acc = new Account(
            options.Value.CloudName,
            options.Value.ApiKey,
            options.Value.ApiSecret
            );


            _cloudinary = new Cloudinary(acc);
        }


        public string UploadImage(byte[] imageBytes, string fileName)
        {
            using var stream = new MemoryStream(imageBytes);
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(fileName, stream)
            };


            var result = _cloudinary.Upload(uploadParams);
            return result.SecureUrl.ToString();
        }
        public async Task<string> UploadProductImage(IFormFile file)
        {
            using var stream = file.OpenReadStream();

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = "products"
            };

            var result = await _cloudinary.UploadAsync(uploadParams);

            if (result.Error != null)
                throw new Exception(result.Error.Message);

            return result.SecureUrl.ToString();
        }



    }
}