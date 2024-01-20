using System.Linq.Expressions;

namespace PetHotel.IServices
{
    public interface IRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null);

        Task<T?> GetAsync(Expression<Func<T, bool>> filter);

        Task CreateAsync(T pet);

        Task UpdateAsync(T pet);

        Task DeleteAsync(T pet);

        Task SaveAsync();
    }
}
