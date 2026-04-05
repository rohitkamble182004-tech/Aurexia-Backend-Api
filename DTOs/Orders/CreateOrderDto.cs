using System;
using System.Collections.Generic;

namespace Fashion.Api.DTOs.Orders
{
    public class CreateOrderDto
    {
        public Guid UserId { get; set; }

        public List<CreateOrderItemDto> Items { get; set; } = new();
    }

    public class CreateOrderItemDto
    {
        public Guid VariantId { get; set; }   // ✅ Changed
        public int Quantity { get; set; }
    }
}