using Fashion.Api.Core.Entities;

namespace Fashion.Api.Core.Interfaces
{
    public interface IDropService
    {
        // =====================
        // STOREFRONT (ASYNC, NO TRACKING)
        // =====================

        Task<IEnumerable<Drop>> GetHomeDropsAsync();
        Task<Drop?> GetBySlugAsync(string slug);
        Task<IEnumerable<Drop>> GetNavDropsAsync();

        // =====================
        // ADMIN (TRACKING OK)
        // =====================

        Task<IEnumerable<Drop>> GetAllAsync();
        Task<Drop> CreateAsync(string name, string slug);
        Task DeleteAsync(Guid id);
        Task<Drop?> GetByIdAsync(Guid id);
    }
}
