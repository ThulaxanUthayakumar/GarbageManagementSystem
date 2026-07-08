using GarbageManagementSystem.API.Data;
using GarbageManagementSystem.API.Models.Entities;
using GarbageManagementSystem.API.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace GarbageManagementSystem.API.Repositories;

public interface ICollectionRequestRepository : IGenericRepository<CollectionRequest>
{
    Task<(List<CollectionRequest> Items, int TotalCount)> GetPagedAsync(
        int pageNumber, int pageSize, string? searchTerm, RequestStatus? status);
    Task<List<CollectionRequest>> GetByResidentIdAsync(int residentId);
    Task<CollectionRequest?> GetByIdWithDetailsAsync(int id);
    Task<int> CountByStatusAsync(RequestStatus status);
    Task<int> CountByResidentAndStatusAsync(int residentId, RequestStatus status);
    Task<List<CollectionRequest>> GetForDateRangeAsync(DateTime from, DateTime to);
    Task<List<CollectionRequest>> GetAllWithDetailsAsync();
}

public class CollectionRequestRepository : GenericRepository<CollectionRequest>, ICollectionRequestRepository
{
    public CollectionRequestRepository(ApplicationDbContext context) : base(context)
    {
    }

    private IQueryable<CollectionRequest> QueryWithDetails() =>
        Context.CollectionRequests
            .Include(cr => cr.Resident).ThenInclude(r => r!.ApplicationUser)
            .Include(cr => cr.WasteCategory);

    public async Task<(List<CollectionRequest> Items, int TotalCount)> GetPagedAsync(
        int pageNumber, int pageSize, string? searchTerm, RequestStatus? status)
    {
        var query = QueryWithDetails();

        if (status.HasValue)
        {
            query = query.Where(cr => cr.Status == status.Value);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim().ToLower();
            query = query.Where(cr =>
                cr.Resident!.ApplicationUser!.FullName.ToLower().Contains(term) ||
                cr.WasteCategory!.Name.ToLower().Contains(term) ||
                cr.Resident.Zone.ToLower().Contains(term));
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(cr => cr.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<List<CollectionRequest>> GetByResidentIdAsync(int residentId)
    {
        return await QueryWithDetails()
            .Where(cr => cr.ResidentId == residentId)
            .OrderByDescending(cr => cr.CreatedAt)
            .ToListAsync();
    }

    public async Task<CollectionRequest?> GetByIdWithDetailsAsync(int id)
    {
        return await QueryWithDetails().FirstOrDefaultAsync(cr => cr.Id == id);
    }

    public async Task<int> CountByStatusAsync(RequestStatus status)
    {
        return await Context.CollectionRequests.CountAsync(cr => cr.Status == status);
    }

    public async Task<int> CountByResidentAndStatusAsync(int residentId, RequestStatus status)
    {
        return await Context.CollectionRequests.CountAsync(cr => cr.ResidentId == residentId && cr.Status == status);
    }

    public async Task<List<CollectionRequest>> GetForDateRangeAsync(DateTime from, DateTime to)
    {
        return await Context.CollectionRequests
            .Include(cr => cr.WasteCategory)
            .Where(cr => cr.CreatedAt >= from && cr.CreatedAt < to)
            .ToListAsync();
    }

    public async Task<List<CollectionRequest>> GetAllWithDetailsAsync()
    {
        return await QueryWithDetails().OrderByDescending(cr => cr.CreatedAt).ToListAsync();
    }
}
