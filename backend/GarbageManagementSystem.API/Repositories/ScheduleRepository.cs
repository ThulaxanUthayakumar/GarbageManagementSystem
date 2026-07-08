using GarbageManagementSystem.API.Data;
using GarbageManagementSystem.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace GarbageManagementSystem.API.Repositories;

public interface IScheduleRepository : IGenericRepository<CollectionSchedule>
{
    Task<(List<CollectionSchedule> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, string? zone);
    Task<List<CollectionSchedule>> GetByZoneAsync(string zone);
    Task<CollectionSchedule?> GetByIdWithDetailsAsync(int id);
    Task<int> CountUpcomingAsync();
}

public class ScheduleRepository : GenericRepository<CollectionSchedule>, IScheduleRepository
{
    public ScheduleRepository(ApplicationDbContext context) : base(context)
    {
    }

    private IQueryable<CollectionSchedule> QueryWithDetails() =>
        Context.CollectionSchedules.Include(cs => cs.WasteCategory);

    public async Task<(List<CollectionSchedule> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, string? zone)
    {
        var query = QueryWithDetails();

        if (!string.IsNullOrWhiteSpace(zone))
        {
            query = query.Where(cs => cs.Zone.ToLower() == zone.Trim().ToLower());
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(cs => cs.ScheduledDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<List<CollectionSchedule>> GetByZoneAsync(string zone)
    {
        return await QueryWithDetails()
            .Where(cs => cs.Zone.ToLower() == zone.Trim().ToLower())
            .OrderBy(cs => cs.ScheduledDate)
            .ToListAsync();
    }

    public async Task<CollectionSchedule?> GetByIdWithDetailsAsync(int id)
    {
        return await QueryWithDetails().FirstOrDefaultAsync(cs => cs.Id == id);
    }

    public async Task<int> CountUpcomingAsync()
    {
        return await Context.CollectionSchedules.CountAsync(cs =>
            cs.ScheduledDate >= DateTime.UtcNow.Date && cs.Status == Models.Enums.ScheduleStatus.Scheduled);
    }
}
