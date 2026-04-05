namespace Fashion.Api.DTOs.Products
{
    public class ProductResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public string Slug { get; internal set; }
        public List<ProductVariantDto> Variants { get; set; } = new();
    }
}
