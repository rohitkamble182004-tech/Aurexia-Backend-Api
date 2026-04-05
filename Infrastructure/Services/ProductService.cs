using Fashion.Api.Core.Entities;
using Fashion.Api.Core.Interfaces;
using Fashion.Api.DTOs.Orders;
using Fashion.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Fashion.Api.Infrastructure.Services
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _db;

        public ProductService(AppDbContext db)
        {
            _db = db;
        }

        // =====================
        // STOREFRONT (ASYNC, NO TRACKING)
        // =====================

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _db.Products
                .AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.Drops)
                .ToListAsync();
        }

        public async Task<Product?> GetBySlugAsync(string slug)
        {
            return await _db.Products
                .AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.Drops)
                .Include(p => p.Variants)
                .FirstOrDefaultAsync(p => p.Slug == slug);
        }

        public async Task<IEnumerable<Product>> GetByDropSlugAsync(string slug)
        {
            return await _db.Products
                .AsNoTracking()
                .Include(p => p.Drops)
                .Where(p => p.Drops.Any(d => d.Slug == slug))
                .ToListAsync();
        }

        // =====================
        // ADMIN (TRACKING OK)
        // =====================

        public async Task<Product?> GetByIdAsync(Guid id)
        {
            return await _db.Products
                .Include(p => p.Drops)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task CreateAsync(Product product)
        {
            _db.Products.Add(product);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Product product)
        {
            _db.Products.Update(product);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null) return;

            _db.Products.Remove(product);
            await _db.SaveChangesAsync();
        }

        public async Task<bool> CategoryExistsAsync(Guid categoryId)
        {
            return await _db.Categories.AnyAsync(c => c.Id == categoryId);
        }

        // =====================
        // DROPS
        // =====================

        public async Task AddProductToDropAsync(Guid productId, Guid dropId)
        {
            var product = await _db.Products
                .Include(p => p.Drops)
                .FirstOrDefaultAsync(p => p.Id == productId);

            var drop = await _db.Drops.FindAsync(dropId);

            if (product == null || drop == null) return;

            if (!product.Drops.Any(d => d.Id == dropId))
            {
                product.Drops.Add(drop);
                await _db.SaveChangesAsync();
            }
        }

        public async Task RemoveProductFromDropAsync(Guid productId, Guid dropId)
        {
            var product = await _db.Products
                .Include(p => p.Drops)
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null) return;

            var drop = product.Drops.FirstOrDefault(d => d.Id == dropId);
            if (drop == null) return;

            product.Drops.Remove(drop);
            await _db.SaveChangesAsync();
        }
        public async Task<List<LowStockVariantDto>> GetLowStockVariantsAsync(int threshold)
        {
            return await _db.ProductVariants
                .AsNoTracking()
                .Include(v => v.Product)
                .Where(v => v.StockQuantity < threshold)
                .Select(v => new LowStockVariantDto
                {
                    VariantId = v.Id,
                    ProductName = v.Product.Name,
                    Size = v.Size,
                    Color = v.Color,
                    StockQuantity = v.StockQuantity
                })
                .ToListAsync();
        }

        // =====================
        // PRODUCT VARIANTS
        // =====================

        public async Task CreateVariantAsync(ProductVariant variant)
        {
            _db.ProductVariants.Add(variant);
            await _db.SaveChangesAsync();
        }

        public async Task<List<ProductVariant>> GetVariantsAsync(Guid productId)
        {
            return await _db.ProductVariants
                .Where(v => v.ProductId == productId)
                .ToListAsync();
        }

        public async Task DeleteVariantsAsync(Guid productId)
        {
            var variants = await _db.ProductVariants
                .Where(v => v.ProductId == productId)
                .ToListAsync();

            _db.ProductVariants.RemoveRange(variants);
            await _db.SaveChangesAsync();
        }
    }
}
