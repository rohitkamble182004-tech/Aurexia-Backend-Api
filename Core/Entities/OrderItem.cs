using System;

namespace Fashion.Api.Core.Entities
{
    public class OrderItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid OrderId { get; set; }
        public Order Order { get; set; }

        public Guid ProductId { get; set; }
        public string? VariantName { get; set; }

        public string? ProductImage { get; set; }

        public string ProductName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
    }
}
