using GarbageManagementSystem.API.Data;
using GarbageManagementSystem.API.Models.Entities;
using GarbageManagementSystem.API.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace GarbageManagementSystem.API.Repositories;

public interface IComplaintRepository : IGenericRepository<Complaint>
{
    Task<(List<Complaint> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, string? searchTerm, ComplaintStatus? status);
    Task<List<Complaint>> GetByResidentIdAsync(int residentId);
    Task<Complaint?> GetByIdWithDetailsAsync(int id);
    Task<int> CountByStatusAsync(ComplaintStatus status);
    Task<int> CountByResidentAndStatusAsync(int residentId, ComplaintStatus status);
    Task<List<Complaint>> GetAllWithDetailsAsync();
}

public class ComplaintRepository : GenericRepository<Complaint>, IComplaintRepository
{
    public ComplaintRepository(ApplicationDbContext context) : base(context)
    {
    }

    private IQueryable<Complaint> QueryWithDetails() =>
        Context.Complaints.Include(c => c.Resident).ThenInclude(r => r!.ApplicationUser);

    public async Task<(List<Complaint> Items, int TotalCount)> GetPagedAsync(
        int pageNumber, int pageSize, string? searchTerm, ComplaintStatus? status)
    {
        var query = QueryWithDetails();

        if (status.HasValue)
        {
            query = query.Where(c => c.Status == status.Value);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim().ToLower();
            query = query.Where(c =>
                c.Subject.ToLower().Contains(term) ||
                c.Resident!.ApplicationUser!.FullName.ToLower().Contains(term));
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<List<Complaint>> GetByResidentIdAsync(int residentId)
    {
        return await QueryWithDetails()
            .Where(c => c.ResidentId == residentId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<Complaint?> GetByIdWithDetailsAsync(int id)
    {
        return await QueryWithDetails().FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<int> CountByStatusAsync(ComplaintStatus status)
    {
        return await Context.Complaints.CountAsync(c => c.Status == status);
    }

    public async Task<int> CountByResidentAndStatusAsync(int residentId, ComplaintStatus status)
    {
        return await Context.Complaints.CountAsync(c => c.ResidentId == residentId && c.Status == status);
    }

    public async Task<List<Complaint>> GetAllWithDetailsAsync()
    {
        return await QueryWithDetails().OrderByDescending(c => c.CreatedAt).ToListAsync();
    }
}
