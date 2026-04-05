using Fashion.Api.Core.Entities;
using Fashion.Api.Core.Interfaces;
using Fashion.Api.DTOs.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("api/orders")]
[Authorize] // 🔒 Require login
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost("my-orders")]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto)
    {
        if (dto.Items == null || !dto.Items.Any())
            return BadRequest(new { message = "Order must contain at least one item." });

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        try
        {
            var order = await _orderService.CreateOrderAsync(
                userId,
                dto.Items
            );

            return Ok(new
            {
                order.Id,
                order.TotalAmount,
                order.OrderStatus,
                order.CreatedAt
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                message = ex.Message,
                inner = ex.InnerException?.Message
            });
        }
    }

    [HttpGet("my-orders")]
    public async Task<IActionResult> GetMyOrders()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var orders = await _orderService.GetMyOrdersAsync(userId);

        return Ok(orders);
    }
}