using Fashion.Api.Core.Entities;
using Fashion.Api.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fashion.Api.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _service;

        public CategoriesController(ICategoryService service)
        {
            _service = service;
        }

        // =====================
        // GET /api/categories (public)
        // =====================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _service.GetAllAsync();

            return Ok(categories.Select(c => new
            {
                id = c.Id,
                name = c.Name,
                slug = c.Slug
            }));
        }

        // =====================
        // POST /api/categories (admin only)
        // =====================
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] string name)
        {
            var category = new Category
            {
                Name = name,
                Slug = name.ToLowerInvariant().Replace(" ", "-")
            };

            await _service.CreateAsync(category);
            return Ok(new { id = category.Id });
        }
    }
}
