using Fashion.Api.Core.Entities;
using Fashion.Api.Core.Enums;
using Fashion.Api.Core.Interfaces;
using Fashion.Api.DTOs.Admin;
using Fashion.Api.DTOs.Orders;
using Fashion.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Fashion.Api.Infrastructure.Services
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;

        public OrderService(AppDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // CREATE ORDER (VARIANT-BASED + SAFE)
        // ==========================================
        public async Task<Order> CreateOrderAsync(
            Guid userId,
            List<CreateOrderItemDto> items)
        {
            if (items == null || !items.Any())
                throw new ArgumentException("Order must contain at least one item.");

            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var order = new Order
                {
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow,
                    OrderStatus = OrderStatus.Processing
                };

                foreach (var item in items)
                {
                    if (item.Quantity <= 0)
                        throw new Exception("Quantity must be greater than zero.");

                    var variant = await _context.ProductVariants
                        .Include(v => v.Product)
                        .FirstOrDefaultAsync(v => v.Id == item.VariantId);

                    if (variant == null)
                        throw new Exception($"Variant {item.VariantId} not found.");

                    if (variant.StockQuantity < item.Quantity)
                        throw new Exception(
                            $"Not enough stock for {variant.Product.Name} ({variant.Size}/{variant.Color}).");

                    var unitPrice = variant.CostPrice ?? variant.Product.Price;

                    // Add order item snapshot
                    order.Items.Add(new OrderItem
                    {
                        ProductId = variant.ProductId,
                        ProductName = variant.Product.Name,
                        VariantName = $"{variant.Size} / {variant.Color}",
                        ProductImage = variant.Product.ImageUrl,
                        UnitPrice = unitPrice,
                        Quantity = item.Quantity
                    });

                    // Deduct stock
                    variant.StockQuantity -= item.Quantity;
                }

                order.TotalAmount = order.Items.Sum(i => i.UnitPrice * i.Quantity);

                _context.Orders.Add(order);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return order;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                Console.WriteLine("========== SAVE ERROR ==========");
                Console.WriteLine("MAIN ERROR: " + ex.Message);

                if (ex.InnerException != null)
                    Console.WriteLine("INNER ERROR: " + ex.InnerException.Message);

                Console.WriteLine("================================");

                throw;
            }
        }

        // ==========================================
        // GET SINGLE ORDER
        // ==========================================
        public async Task<Order?> GetByIdAsync(Guid id)
        {
            return await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        // ==========================================
        // GET USER ORDERS
        // ==========================================
        public async Task<List<Order>> GetUserOrdersAsync(Guid userId)
        {
            return await _context.Orders
                .Include(o => o.Items)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        // ==========================================
        // UPDATE STATUS
        // ==========================================
        public async Task UpdateStatusAsync(Guid orderId, OrderStatus newStatus)
        {
            var order = await _context.Orders.FindAsync(orderId);

            if (order == null)
                throw new Exception("Order not found");

            if (order.OrderStatus == newStatus)
                return; // No change needed

            if (!IsValidStatusTransition(order.OrderStatus, newStatus))
                throw new InvalidOperationException(
                    $"Invalid status transition from {order.OrderStatus} to {newStatus}");

            order.OrderStatus = newStatus;

            // 🔹 Auto-set timestamps
            if (newStatus == OrderStatus.Shipped)
                order.ShippedAt = DateTime.UtcNow;

            if (newStatus == OrderStatus.Delivered)
                order.DeliveredAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        // ==========================================
        // REVENUE ANALYTICS
        // ==========================================
        public async Task<decimal> GetTotalRevenueAsync()
        {
            return await _context.Orders
                .Where(o => o.OrderStatus == OrderStatus.Delivered)
                .SumAsync(o => o.TotalAmount);
        }

        public async Task<decimal> GetTodayRevenueAsync()
        {
            var today = DateTime.UtcNow.Date;

            return await _context.Orders
                .Where(o => o.OrderStatus == OrderStatus.Delivered &&
                            o.CreatedAt.Date == today)
                .SumAsync(o => o.TotalAmount);
        }

        // ==========================================
        // STATUS TRANSITION RULES
        // ==========================================
        private bool IsValidStatusTransition(OrderStatus current, OrderStatus next)
        {
            return current switch
            {
                OrderStatus.Processing => next == OrderStatus.Confirmed || next == OrderStatus.Cancelled,
                OrderStatus.Confirmed => next == OrderStatus.Packed || next == OrderStatus.Cancelled,
                OrderStatus.Packed => next == OrderStatus.Shipped,
                OrderStatus.Shipped => next == OrderStatus.Delivered,
                OrderStatus.Delivered => false,
                OrderStatus.Cancelled => false,
                _ => false
            };
        }

        // =====================
        // DASHBOARD (ADMIN)
        // =====================
        public async Task<DashboardStatsDto> GetDashboardStatsAsync()
        {
            var today = DateTime.UtcNow.Date;

            // 1️⃣ Total revenue
            var totalRevenue = await _context.Orders
                .Where(o => o.OrderStatus == OrderStatus.Delivered)
                .SumAsync(o => (decimal?)o.TotalAmount) ?? 0;

            // 2️⃣ Today revenue
            var todayRevenue = await _context.Orders
                .Where(o => o.OrderStatus == OrderStatus.Delivered &&
                            o.CreatedAt.Date == today)
                .SumAsync(o => (decimal?)o.TotalAmount) ?? 0;

            // 3️⃣ Total orders
            var totalOrders = await _context.Orders.CountAsync();

            // 4️⃣ Pending orders
            var pendingOrders = await _context.Orders
                .Where(o => o.OrderStatus == OrderStatus.Processing)
                .CountAsync();

            return new DashboardStatsDto
            {
                TotalRevenue = totalRevenue,
                TodayRevenue = todayRevenue,
                TotalOrders = totalOrders,
                PendingOrders = pendingOrders
            };
        }

        // ==========================================
        // MY ORDER UPDATE STATUS
        // ==========================================
        public async Task<List<MyOrderDto>> GetMyOrdersAsync(Guid userId)
        {
            return await _context.Orders
                .AsNoTracking()
                .Include(o => o.Items)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => new MyOrderDto
                {
                    Id = o.Id,
                    CreatedAt = o.CreatedAt,
                    TotalAmount = o.TotalAmount,
                    OrderStatus = o.OrderStatus.ToString(),
                    TrackingNumber = o.TrackingNumber,
                    ShippingCompany = o.ShippingCompany,
                    Items = o.Items.Select(i => new MyOrderItemDto
                    {
                        ProductName = i.ProductName,
                        UnitPrice = i.UnitPrice,
                        Quantity = i.Quantity
                    }).ToList()
                })
                .ToListAsync();
        }

        // =====================
        // UpdateTracking (ADMIN)
        // =====================

        public async Task UpdateTrackingAsync(Guid orderId, UpdateTrackingDto dto)
        {
            var order = await _context.Orders.FindAsync(orderId);

            if (order == null)
                throw new Exception("Order not found");

            order.TrackingNumber = dto.TrackingNumber;
            order.ShippingCompany = dto.ShippingCompany;

            await _context.SaveChangesAsync();
        }
        private readonly INotificationService _notificationService;

        public OrderService(AppDbContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        // =====================
        // GET ORDERS SUMMARY (ADMIN)
        // =====================
        public async Task<List<AdminOrderSummaryDto>> GetRecentOrdersAsync(int limit)
        {
            return await _context.Orders
                .AsNoTracking()
                .OrderByDescending(o => o.CreatedAt)
                .Take(limit)
                .Select(o => new AdminOrderSummaryDto
                {
                    Id = o.Id,
                    TotalAmount = o.TotalAmount,
                    OrderStatus = o.OrderStatus.ToString(),
                    CreatedAt = o.CreatedAt,
                    TrackingNumber = o.TrackingNumber,
                    ShippingCompany = o.ShippingCompany,
                    UserId = o.UserId
                })
                .ToListAsync();
        }
    }
}