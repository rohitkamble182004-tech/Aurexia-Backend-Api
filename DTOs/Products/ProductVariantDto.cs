namespace Fashion.Api.DTOs.Products
{
    public class ProductVariantDto
    {
        public Guid Id { get; set; }
        public string Size { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public int StockQuantity { get; set; }
    }
}
