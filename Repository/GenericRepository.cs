using Microsoft.EntityFrameworkCore;
using MyStoreRatingsApi.Data;
using System.Collections.Generic;

namespace MyStoreRatingsApi.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _db;
        protected readonly DbSet<T> _set;
        public GenericRepository(ApplicationDbContext db)
        {
            _db = db;
            _set = db.Set<T>();
        }

        public virtual async Task AddAsync(T entity) => await _set.AddAsync(entity);
        public virtual void Delete(T entity) => _set.Remove(entity);
        public virtual async Task<IEnumerable<T>> GetAllAsync() => await _set.ToListAsync();
        public virtual async Task<T?> GetByIdAsync(object id) => await _set.FindAsync(id);
        public virtual void Update(T entity) => _set.Update(entity);
        public virtual async Task SaveChangesAsync() => await _db.SaveChangesAsync();
        public IQueryable<T> Query() => _set.AsQueryable();
    }
}
