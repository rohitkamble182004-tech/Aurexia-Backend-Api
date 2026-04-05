using Fashion.Api.Core.Entities;
using Fashion.Api.Core.Interfaces;
using Fashion.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Fashion.Api.Infrastructure.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly AppDbContext _db;

        public CategoryService(AppDbContext db)
        {
            _db = db;
        }

        // =====================
        // STOREFRONT / ADMIN
        // =====================

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _db.Categories
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Category?> GetByIdAsync(Guid id)
        {
            return await _db.Categories
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _db.Categories
                .AnyAsync(c => c.Id == id);
        }

        public async Task CreateAsync(Category category)
        {
            _db.Categories.Add(category);
            await _db.SaveChangesAsync();
        }
    }
}
