using Fashion.Api.Core.Entities;
using Fashion.Api.Core.Enums;
using Fashion.Api.DTOs.Admin;
using Fashion.Api.DTOs.Orders;

namespace Fashion.Api.Core.Interfaces
{
    public interface IOrderService
    {
        // Create Order
        Task<Order> CreateOrderAsync(Guid userId, List<CreateOrderItemDto> items);

        // Get single order
        Task<Order?> GetByIdAsync(Guid id);

        // Get user orders
        Task<List<Order>> GetUserOrdersAsync(Guid userId);

        // Update order status
        Task UpdateStatusAsync(Guid orderId, OrderStatus newStatus);

        // Revenue analytics
        Task<decimal> GetTotalRevenueAsync();
        Task<decimal> GetTodayRevenueAsync();

        //Dashboard (ADMIN)
        Task<DashboardStatsDto> GetDashboardStatsAsync();

        // Upcoming Orders User
        Task<List<MyOrderDto>> GetMyOrdersAsync(Guid userId);

        // Update tracking (ADMIN)
        Task UpdateTrackingAsync(Guid orderId, UpdateTrackingDto dto);

        // Get Orders Summary (ADMIN)
        Task<List<AdminOrderSummaryDto>> GetRecentOrdersAsync(int limit);

    }
}