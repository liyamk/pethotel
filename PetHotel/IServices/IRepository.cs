using System.Linq.Expressions;

namespace PetHotel.IServices
{
    public interface IRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null);

        Task<T?> GetAsync(Expression<Func<T, bool>> filter);

        Task CreateAsync(T entity);

        Task UpdateAsync(T entity);

        Task DeleteAsync(T entity);

        Task SaveAsync();
    }
}
