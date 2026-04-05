using Fashion.Api.Core.Entities;
using Fashion.Api.DTOs.Orders;

namespace Fashion.Api.Core.Interfaces
{
    public interface IProductService
    {
        // =====================
        // STOREFRONT (ASYNC, NO TRACKING)
        // =====================

        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product?> GetBySlugAsync(string slug);
        Task<IEnumerable<Product>> GetByDropSlugAsync(string slug);

        // =====================
        // ADMIN (TRACKING OK)
        // =====================

        Task<Product?> GetByIdAsync(Guid id);
        Task CreateAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(Guid id);
        Task<bool> CategoryExistsAsync(Guid categoryId);
        // NEW
        Task CreateVariantAsync(ProductVariant variant);
        Task<List<ProductVariant>> GetVariantsAsync(Guid productId);
        Task DeleteVariantsAsync(Guid productId);

        // =====================
        // DROPS
        // =====================

        Task AddProductToDropAsync(Guid productId, Guid dropId);
        Task RemoveProductFromDropAsync(Guid productId, Guid dropId);
        Task<List<LowStockVariantDto>> GetLowStockVariantsAsync(int threshold);
    }
}
