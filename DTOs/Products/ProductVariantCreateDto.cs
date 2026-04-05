using System.Text.Json.Serialization;

namespace Fashion.Api.DTOs.Products
{
    public class ProductVariantCreateDto
    {
        [JsonPropertyName("size")]
        public string Size { get; set; }

        [JsonPropertyName("color")]
        public string Color { get; set; }

        [JsonPropertyName("stockQuantity")]
        public int StockQuantity { get; set; }
    }
}