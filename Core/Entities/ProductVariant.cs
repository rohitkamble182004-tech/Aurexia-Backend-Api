namespace Fashion.Api.Core.Entities
{
    public class ProductVariant
    {
        public Guid Id { get; set; }

        public Guid ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public string? Size { get; set; }
        public string? Color { get; set; }

        public string SKU { get; set; } = "";

        public int StockQuantity { get; set; }

        public decimal? CostPrice { get; set; }   // for brand future
    }
}
