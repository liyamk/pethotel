using Microsoft.EntityFrameworkCore;
using PetHotel.IServices;
using PetHotel.Models;
using System.Linq.Expressions;

namespace PetHotel.Services
{

    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly PetHotelDbContext _db;
        private readonly DbSet<T> _dbSet;
        public Repository(PetHotelDbContext appDbContext)
        {
            _db = appDbContext;
            _dbSet = appDbContext.Set<T>();
        }

        public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null)
        {
            IQueryable<T> query = _dbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await query.ToListAsync();
        }

        public async Task<T?> GetAsync(Expression<Func<T, bool>> filter)
        {
            IQueryable<T> query = _dbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await query.FirstOrDefaultAsync();
        }

        public async Task CreateAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await SaveAsync();
        }

        public async Task DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
            await SaveAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            _db.Update(entity);
            await SaveAsync();
        }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
