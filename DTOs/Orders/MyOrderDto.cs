namespace Fashion.Api.DTOs.Orders
{
    public class MyOrderDto
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal TotalAmount { get; set; }
        public string OrderStatus { get; set; } = string.Empty;
        public string? TrackingNumber { get; set; }
        public string? ShippingCompany { get; set; }

        public List<MyOrderItemDto> Items { get; set; } = new();
    }

    public class MyOrderItemDto
    {
        public string ProductName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
    }
}
