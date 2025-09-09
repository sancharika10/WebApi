using MyStoreRatingsApi.Models;

namespace MyStoreRatingsApi.Repositories
{
    public interface IRatingRepository : IGenericRepository<Rating>
    {
        Task<Rating?> GetByUserAndStoreAsync(string userId, int storeId);
        Task<IEnumerable<Rating>> GetByStoreAsync(int storeId);
    }
}
