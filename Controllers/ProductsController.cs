using Fashion.Api.Core.Interfaces;
using Fashion.Api.DTOs.Products;
using Microsoft.AspNetCore.Mvc;

namespace Fashion.Api.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _service;

        public ProductsController(IProductService service)
        {
            _service = service;
        }

        // =====================
        // GET /api/products
        // GET /api/products?drop=slug
        // GET /api/products?search=query
        // =====================
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? drop,
            [FromQuery] string? search
        )
        {
            IEnumerable<Core.Entities.Product> products;

            if (!string.IsNullOrWhiteSpace(drop))
            {
                products = await _service.GetByDropSlugAsync(drop);
            }
            else
            {
                products = await _service.GetAllAsync();
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                products = products.Where(p =>
                    p.Name.Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            return Ok(products.Select(p => new ProductResponseDto
            {
                Id = p.Id,
                Name = p.Name,
                Slug = p.Slug,
                Price = p.Price,
                ImageUrl = p.ImageUrl
            }));
        }

        // =====================
        // GET /api/products/{slug}
        // =====================
        [HttpGet("{slug}")]
        public async Task<IActionResult> GetBySlug(string slug)
        {
            var product = await _service.GetBySlugAsync(slug);

            if (product == null)
                return NotFound();

            return Ok(new ProductResponseDto
            {
                Id = product.Id,
                Name = product.Name,
                Slug = product.Slug,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                Variants = product.Variants.Select(v => new ProductVariantDto // ✅ add this
                {
                    Id = v.Id,
                    Size = v.Size,
                    Color = v.Color,
                    StockQuantity = v.StockQuantity
                }).ToList()
            });
        }
    }
}
