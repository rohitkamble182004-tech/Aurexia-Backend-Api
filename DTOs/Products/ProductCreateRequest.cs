using Microsoft.AspNetCore.Http;

namespace Fashion.Api.DTOs.Products
{
    public class ProductCreateRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public Guid CategoryId { get; set; }
        public IFormFile? Image { get; set; }
        public string VariantsJson { get; set; } = string.Empty;
    }
}
