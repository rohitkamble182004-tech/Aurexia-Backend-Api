namespace Fashion.Api.DTOs.Admin
{
    public class DashboardStatsDto
    {
        public decimal TotalRevenue { get; set; }
        public decimal TodayRevenue { get; set; }
        public int TotalOrders { get; set; }
        public int PendingOrders { get; set; }
    }
}
