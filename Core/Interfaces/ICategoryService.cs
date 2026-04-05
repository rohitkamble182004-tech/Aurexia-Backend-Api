using Fashion.Api.Core.Entities;

namespace Fashion.Api.Core.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetAllAsync();
        Task<Category?> GetByIdAsync(Guid id);
        Task CreateAsync(Category category);
        Task<bool> ExistsAsync(Guid id);
    }
}
