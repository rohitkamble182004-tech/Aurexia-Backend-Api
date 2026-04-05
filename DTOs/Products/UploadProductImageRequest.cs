using Microsoft.AspNetCore.Http;

namespace Fashion.Api.DTOs.Products
{
    public class UploadProductImageRequest
    {
        public IFormFile Image { get; set; } = null!;
    }
}
