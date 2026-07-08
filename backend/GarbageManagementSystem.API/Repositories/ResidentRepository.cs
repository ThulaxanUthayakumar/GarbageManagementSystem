using GarbageManagementSystem.API.Data;
using GarbageManagementSystem.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace GarbageManagementSystem.API.Repositories;

public interface IResidentRepository : IGenericRepository<Resident>
{
    Task<(List<Resident> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, string? searchTerm);
    Task<Resident?> GetByIdWithUserAsync(int id);
    Task<Resident?> GetByApplicationUserIdAsync(string applicationUserId);
    Task<int> CountActiveAsync();
}

public class ResidentRepository : GenericRepository<Resident>, IResidentRepository
{
    public ResidentRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<(List<Resident> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, string? searchTerm)
    {
        var query = Context.Residents
            .Include(r => r.ApplicationUser)
            .Include(r => r.CollectionRequests)
            .Include(r => r.Complaints)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim().ToLower();
            query = query.Where(r =>
                r.ApplicationUser!.FullName.ToLower().Contains(term) ||
                r.ApplicationUser!.Email!.ToLower().Contains(term) ||
                r.Zone.ToLower().Contains(term) ||
                r.City.ToLower().Contains(term));
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(r => r.DateJoined)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<Resident?> GetByIdWithUserAsync(int id)
    {
        return await Context.Residents
            .Include(r => r.ApplicationUser)
            .Include(r => r.CollectionRequests)
            .Include(r => r.Complaints)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<Resident?> GetByApplicationUserIdAsync(string applicationUserId)
    {
        return await Context.Residents
            .Include(r => r.ApplicationUser)
            .FirstOrDefaultAsync(r => r.ApplicationUserId == applicationUserId);
    }

    public async Task<int> CountActiveAsync()
    {
        return await Context.Residents.CountAsync(r => r.IsActive);
    }
}
