using Microsoft.EntityFrameworkCore;
using MyStoreRatingsApi.Data;
using MyStoreRatingsApi.Models;

namespace MyStoreRatingsApi.Repositories
{
    public class StoreRepository : GenericRepository<Store>, IStoreRepository
    {
        public StoreRepository(ApplicationDbContext db) : base(db) { }

        public async Task<Store?> GetWithRatingsAsync(int id)
        {
            return await _db.Stores.Include(s => s.Ratings).FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<Store>> FilterAsync(string? name, string? address, string? sortBy, bool desc)
        {
            var q = _db.Stores.Include(s => s.Ratings).AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
                q = q.Where(s => s.Name.Contains(name));
            if (!string.IsNullOrWhiteSpace(address))
                q = q.Where(s => s.Address != null && s.Address.Contains(address));

            q = sortBy?.ToLower() switch
            {
                "name" => desc ? q.OrderByDescending(s => s.Name) : q.OrderBy(s => s.Name),
                "email" => desc ? q.OrderByDescending(s => s.Email) : q.OrderBy(s => s.Email),
                _ => q.OrderBy(s => s.Id)
            };

            return await q.ToListAsync();
        }
    }
}
