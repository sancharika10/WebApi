using Microsoft.EntityFrameworkCore;
using MyStoreRatingsApi.Data;
using MyStoreRatingsApi.Models;

namespace MyStoreRatingsApi.Repositories
{
    public class RatingRepository : GenericRepository<Rating>, IRatingRepository
    {
        public RatingRepository(ApplicationDbContext db) : base(db) { }

        public async Task<Rating?> GetByUserAndStoreAsync(string userId, int storeId)
        {
            return await _db.Ratings.FirstOrDefaultAsync(r => r.UserId == userId && r.StoreId == storeId);
        }

        public async Task<IEnumerable<Rating>> GetByStoreAsync(int storeId)
        {
            return await _db.Ratings.Where(r => r.StoreId == storeId).ToListAsync();
        }
    }
}
