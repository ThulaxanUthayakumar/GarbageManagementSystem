using System.Linq.Expressions;
using GarbageManagementSystem.API.Data;
using Microsoft.EntityFrameworkCore;

namespace GarbageManagementSystem.API.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly ApplicationDbContext Context;
    protected readonly DbSet<T> DbSet;

    public GenericRepository(ApplicationDbContext context)
    {
        Context = context;
        DbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(int id) => await DbSet.FindAsync(id);

    public async Task<List<T>> GetAllAsync() => await DbSet.ToListAsync();

    public async Task<T> AddAsync(T entity)
    {
        await DbSet.AddAsync(entity);
        return entity;
    }

    public void Update(T entity) => DbSet.Update(entity);

    public void Delete(T entity) => DbSet.Remove(entity);

    public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate) => await DbSet.AnyAsync(predicate);

    public async Task<int> SaveChangesAsync() => await Context.SaveChangesAsync();
}
