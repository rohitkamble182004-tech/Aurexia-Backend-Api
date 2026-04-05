using Fashion.Api.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace Fashion.Api.Controllers
{
    [ApiController]
    [Route("api/storefront")]
    public class StorefrontController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IDropService _dropService;

        public StorefrontController(
            IProductService productService,
            IDropService dropService)
        {
            _productService = productService;
            _dropService = dropService;
        }

        // ============================
        // HOME PAGE (ACTIVE DROPS)
        // GET /api/storefront/home
        // ============================
        [HttpGet("home")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> Home()
        {
            var drops = await _dropService.GetHomeDropsAsync();

            return Ok(drops.Select(d => new
            {
                d.Id,
                d.Name,
                d.Slug,
                products = d.Products.Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Slug,
                    p.Price,
                    p.ImageUrl
                })
            }));
        }

        // ============================
        // DROP PAGE
        // GET /api/storefront/drops/{slug}
        // ============================
        [HttpGet("drops/{slug}")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> Drop(string slug)
        {
            var drop = await _dropService.GetBySlugAsync(slug);
            if (drop == null)
                return NotFound();

            return Ok(new
            {
                drop.Id,
                drop.Name,
                drop.Slug,
                products = drop.Products.Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Slug,
                    p.Price,
                    p.ImageUrl
                })
            });
        }

        // ============================
        // NAV DROPS
        // GET /api/storefront/nav
        // ============================
        [HttpGet("nav")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> Nav()
        {
            var drops = await _dropService.GetNavDropsAsync();

            return Ok(drops.Select(parent => new
            {
                parent.Id,
                parent.Name,
                parent.Slug,
                children = parent.Children
                    .Where(c => c.IsActive)
                    .OrderBy(c => c.SortOrder)
                    .Select(c => new
                    {
                        c.Id,
                        c.Name,
                        c.Slug,
                        is_editorial = c.IsEditorial
                    })
            }));
        }


        // ============================
        // BEST SELLERS
        // GET /api/storefront/best-sellers
        // ============================
        [HttpGet("best-sellers")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> BestSellers()
        {
            var products = await _productService.GetByDropSlugAsync("best-sellers");

            return Ok(products.Select(p => new
            {
                p.Id,
                p.Name,
                p.Slug,
                p.Price,
                p.ImageUrl
            }));
        }
    }
}
