using Fashion.Api.Core.Entities;
using Fashion.Api.Core.Enums;
using Fashion.Api.Core.Interfaces;
using Fashion.Api.DTOs.Admin;
using Fashion.Api.DTOs.Orders;
using Fashion.Api.DTOs.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "admin")]
public class AdminController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly IImageService _imageService;
    private readonly IDropService _dropService;
    private readonly IOrderService _orderService;

    public AdminController(
        IProductService productService,
        IImageService imageService,
        IDropService dropService,
        IOrderService orderService)
    {
        _productService = productService;
        _imageService = imageService;
        _dropService = dropService;
        _orderService = orderService;
    }

    // =====================
    // PRODUCTS
    // =====================

    [HttpGet("products")]
    public async Task<IActionResult> GetProducts()
    {
        var products = await _productService.GetAllAsync();

        return Ok(products.Select(p => new
        {
            p.Id,
            p.Name,
            p.Price,
            p.CategoryId,
            p.ImageUrl
        }));
    }

    [HttpGet("products/{id:guid}")]
    public async Task<IActionResult> GetProduct(Guid id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product == null) return NotFound();

        var variants = await _productService.GetVariantsAsync(id);

        return Ok(new
        {
            product.Id,
            product.Name,
            product.Description,
            product.Price,
            product.CategoryId,
            product.ImageUrl,

            variants = variants.Select(v => new
            {
                v.Id,
                v.Size,
                v.Color,
                v.StockQuantity
            })
        });
    }

    [HttpPost("products")]
    public async Task<IActionResult> CreateProduct([FromForm] ProductCreateRequest request)
    {
        if (!await _productService.CategoryExistsAsync(request.CategoryId))
            return BadRequest("Invalid category");

        var imageUrl = request.Image != null
            ? await _imageService.UploadProductImage(request.Image)
            : null;

        var product = new Product
        {
            Name = request.Name,
            Slug = GenerateSlug(request.Name),
            Description = request.Description,
            Price = request.Price,
            CategoryId = request.CategoryId,
            ImageUrl = imageUrl
        };

        await _productService.CreateAsync(product);

        // Deserialize variants
        var variants = JsonSerializer.Deserialize<List<ProductVariantCreateDto>>(
            request.VariantsJson,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        if (variants != null)
        {
            foreach (var v in variants)
            {
                await _productService.CreateVariantAsync(new ProductVariant
                {
                    ProductId = product.Id,
                    SKU = GenerateSku(product.Name, v.Size, v.Color),
                    Size = v.Size,
                    Color = v.Color,
                    StockQuantity = v.StockQuantity
                });
            }
        }

        return Ok(new { product.Id });
    }

    private static string GenerateSku(string name, string size, string color)
    {
        var prefix = name.Substring(0, Math.Min(3, name.Length)).ToUpper();
        var random = Guid.NewGuid().ToString()[..4].ToUpper();

        return $"{prefix}-{size}-{color}-{random}";
    }

    [HttpPut("products/{id:guid}")]
    public async Task<IActionResult> UpdateProduct(Guid id, [FromForm] ProductUpdateDto dto)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product == null)
            return NotFound();

        product.Name = dto.Name;
        product.Description = dto.Description;
        product.Price = dto.Price;
        product.CategoryId = dto.CategoryId;

        if (dto.Image != null)
        {
            var imageUrl = await _imageService.UploadProductImage(dto.Image);
            product.ImageUrl = imageUrl;
        }

        await _productService.UpdateAsync(product);

        // Remove old variants
        await _productService.DeleteVariantsAsync(id);

        // Deserialize new variants
        var variants = JsonSerializer.Deserialize<List<ProductVariantCreateDto>>(
            dto.VariantsJson,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );

        if (variants != null)
        {
            foreach (var v in variants)
            {
                await _productService.CreateVariantAsync(new ProductVariant
                {
                    ProductId = id,
                    SKU = GenerateSku(product.Name, v.Size, v.Color),
                    Size = v.Size,
                    Color = v.Color,
                    StockQuantity = v.StockQuantity
                });
            }
        }

        return NoContent();
    }

    [HttpDelete("products/{id:guid}")]
    public async Task<IActionResult> DeleteProduct(Guid id)
    {
        await _productService.DeleteAsync(id);
        return NoContent();
    }

    // =====================
    // DROPS (ADMIN)
    // =====================

    [HttpGet("drops")]
    public async Task<IActionResult> GetDrops()
    {
        var drops = await _dropService.GetAllAsync();

        return Ok(drops.Select(d => new
        {
            d.Id,
            d.Name,
            d.Slug,
            d.ParentId,
            d.IsActive,
            d.SortOrder
        }));
    }

    [HttpGet("drops/{dropId:guid}/products")]
    public async Task<IActionResult> GetProductsInDrop(Guid dropId)
    {
        var drop = await _dropService.GetByIdAsync(dropId);
        if (drop == null) return NotFound();

        return Ok(drop.Products.Select(p => new
        {
            p.Id,
            p.Name
        }));
    }

    [HttpPost("drops/{dropId:guid}/products/{productId:guid}")]
    public async Task<IActionResult> AddProductToDrop(Guid dropId, Guid productId)
    {
        await _productService.AddProductToDropAsync(productId, dropId);
        return NoContent();
    }

    [HttpDelete("drops/{dropId:guid}/products/{productId:guid}")]
    public async Task<IActionResult> RemoveProductFromDrop(Guid dropId, Guid productId)
    {
        await _productService.RemoveProductFromDropAsync(productId, dropId);
        return NoContent();
    }

    private static string GenerateSlug(string name)
    {
        return name
            .ToLowerInvariant()
            .Trim()
            .Replace(" ", "-");
    }

    // =====================
    // ORDERS (ADMIN)
    // =====================

    [HttpGet("orders")]
    public async Task<IActionResult> GetRecentOrders([FromQuery] int limit = 5)
    {
        var orders = await _orderService.GetRecentOrdersAsync(limit);
        return Ok(orders);
    }
    [HttpPut("orders/{id:guid}/status")]
    public async Task<IActionResult> UpdateOrderStatus(
        Guid id,
        [FromBody] UpdateOrderStatusDto dto)
    {
        try
        {
            await _orderService.UpdateStatusAsync(id, dto.Status);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception)
        {
            return NotFound(new { message = "Order not found" });
        }
    }
    [HttpGet("analytics/revenue")]
    public async Task<IActionResult> GetRevenue()
    {
        var totalRevenue = await _orderService.GetTotalRevenueAsync();
        var todayRevenue = await _orderService.GetTodayRevenueAsync();

        return Ok(new
        {
            totalRevenue,
            todayRevenue
        });
    }
    [HttpGet("low-stock")]
    public async Task<IActionResult> GetLowStock()
    {
        var lowStockVariants = await _productService.GetLowStockVariantsAsync(5);
        return Ok(lowStockVariants);
    }
    [HttpPut("orders/{id:guid}/tracking")]
    public async Task<IActionResult> UpdateTracking(
    Guid id,
    [FromBody] UpdateTrackingDto dto)
    {
        await _orderService.UpdateTrackingAsync(id, dto);
        return NoContent();
    }

    // =====================
    // DASHBOARD (ADMIN)
    // =====================

    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboardStats()
    {
        var stats = await _orderService.GetDashboardStatsAsync();
        return Ok(stats);
    }

}
