namespace Fashion.Api.DTOs.Products
{
    public class ProductUpdateDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public Guid CategoryId { get; set; }

        public IFormFile Image { get; set; } = null!;
        public string VariantsJson { get; set; } = "";
    }
}
