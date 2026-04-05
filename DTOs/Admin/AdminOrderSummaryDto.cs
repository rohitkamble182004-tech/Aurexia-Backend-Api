namespace Fashion.Api.DTOs.Admin
{
    public class AdminOrderSummaryDto
    {
        public Guid Id { get; set; }
        public decimal TotalAmount { get; set; }
        public string OrderStatus { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        public string? TrackingNumber { get; set; }
        public string? ShippingCompany { get; set; }

        public Guid UserId { get; set; }
    }
}
