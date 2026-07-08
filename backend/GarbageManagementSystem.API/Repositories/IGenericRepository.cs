using System.Linq.Expressions;

namespace GarbageManagementSystem.API.Repositories;

/// <summary>
/// Basic CRUD operations shared by every repository. Entity-specific repositories
/// extend this with their own queries (paging, filtering, joins).
/// </summary>
public interface IGenericRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<List<T>> GetAllAsync();
    Task<T> AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
    Task<int> SaveChangesAsync();
}
