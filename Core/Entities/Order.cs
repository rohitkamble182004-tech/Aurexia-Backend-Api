using Fashion.Api.Core.Enums;
using System;
using System.Collections.Generic;

namespace Fashion.Api.Core.Entities
{
    public class Order
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }

        public decimal TotalAmount { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();

        public OrderStatus OrderStatus { get; set; } = OrderStatus.Processing;
        public string? TrackingNumber { get; set; }
        public string? ShippingCompany { get; set; }
        public DateTime? ShippedAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
    }
}