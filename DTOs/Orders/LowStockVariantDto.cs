namespace Fashion.Api.DTOs.Orders
{
    public class LowStockVariantDto
    {
        public Guid VariantId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? Size { get; set; }
        public string? Color { get; set; }
        public int StockQuantity { get; set; }
    }
}
