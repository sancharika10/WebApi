using MyStoreRatingsApi.Models;

namespace MyStoreRatingsApi.Repositories
{
    public interface IStoreRepository : IGenericRepository<Store>
    {
        Task<Store?> GetWithRatingsAsync(int id);
        Task<IEnumerable<Store>> FilterAsync(string? name, string? address, string? sortBy, bool desc);
    }
}
