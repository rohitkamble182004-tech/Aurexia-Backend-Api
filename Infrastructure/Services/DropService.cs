using Fashion.Api.Core.Entities;
using Fashion.Api.Core.Interfaces;
using Fashion.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Fashion.Api.Infrastructure.Services
{
    public class DropService : IDropService
    {
        private readonly AppDbContext _db;

        public DropService(AppDbContext db)
        {
            _db = db;
        }

        // =====================
        // STOREFRONT (ASYNC, NO TRACKING)
        // =====================

        public async Task<IEnumerable<Drop>> GetHomeDropsAsync()
        {
            return await _db.Drops
                .AsNoTracking()
                .Where(d => d.IsActive)
                .OrderBy(d => d.SortOrder)
                .Select(d => new Drop
                {
                    Id = d.Id,
                    Name = d.Name,
                    Slug = d.Slug,
                    IsActive = d.IsActive,
                    SortOrder = d.SortOrder,
                    Products = d.Products.Select(p => new Product
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Slug = p.Slug,
                        Price = p.Price,
                        CategoryId = p.CategoryId,
                        ImageUrl = p.ImageUrl
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task<Drop?> GetBySlugAsync(string slug)
        {
            var drop = await _db.Drops
                .AsNoTracking()
                .Include(d => d.Children)
                    .ThenInclude(c => c.Products)
                .Include(d => d.Products)
                .FirstOrDefaultAsync(d =>
                    d.IsActive &&
                    d.Slug == slug);

            if (drop == null)
                return null;

            // If parent landing page → merge children products
            if (drop.ParentId == null && drop.Children.Any())
            {
                var mergedProducts = drop.Children
                    .Where(c => c.IsActive)
                    .SelectMany(c => c.Products)
                    .GroupBy(p => p.Id) // remove duplicates
                    .Select(g => g.First())
                    .Select(p => new Product
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Slug = p.Slug,
                        Price = p.Price,
                        CategoryId = p.CategoryId,
                        ImageUrl = p.ImageUrl
                    })
                    .ToList();

                drop.Products = mergedProducts;
            }
            else
            {
                // If child → map its own products
                drop.Products = drop.Products
                    .Select(p => new Product
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Slug = p.Slug,
                        Price = p.Price,
                        CategoryId = p.CategoryId,
                        ImageUrl = p.ImageUrl
                    })
                    .ToList();
            }

            return drop;
        }

        public async Task<IEnumerable<Drop>> GetNavDropsAsync()
        {
            return await _db.Drops
                .AsNoTracking()
                .Where(d =>
                    d.IsActive &&
                    d.ShowInNav &&
                    d.ParentId == null        // ✅ ONLY parents
                )
                .OrderBy(d => d.SortOrder)
                .Include(d => d.Children)     // ✅ LOAD dropdown items
                .ToListAsync();
        }


        // =====================
        // ADMIN (TRACKING OK)
        // =====================

        public async Task<IEnumerable<Drop>> GetAllAsync()
        {
            return await _db.Drops
                .OrderBy(d => d.SortOrder)
                .ToListAsync();
        }

        public async Task<Drop> CreateAsync(string name, string slug)
        {
            var drop = new Drop
            {
                Id = Guid.NewGuid(),
                Name = name,
                Slug = slug,
                IsActive = true,
                ShowInNav = true
            };

            _db.Drops.Add(drop);
            await _db.SaveChangesAsync();

            return drop;
        }

        public async Task DeleteAsync(Guid id)
        {
            var drop = await _db.Drops.FindAsync(id);
            if (drop == null) return;

            _db.Drops.Remove(drop);
            await _db.SaveChangesAsync();
        }

        public async Task<Drop?> GetByIdAsync(Guid id)
        {
            return await _db.Drops
                .Include(d => d.Products)
                .FirstOrDefaultAsync(d => d.Id == id);
        }
    }
}
