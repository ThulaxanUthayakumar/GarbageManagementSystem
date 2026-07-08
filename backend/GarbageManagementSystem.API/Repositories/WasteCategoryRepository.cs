using GarbageManagementSystem.API.Data;
using GarbageManagementSystem.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace GarbageManagementSystem.API.Repositories;

public interface IWasteCategoryRepository : IGenericRepository<WasteCategory>
{
    Task<bool> NameExistsAsync(string name, int? excludeId = null);
}

public class WasteCategoryRepository : GenericRepository<WasteCategory>, IWasteCategoryRepository
{
    public WasteCategoryRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<bool> NameExistsAsync(string name, int? excludeId = null)
    {
        return await Context.WasteCategories
            .AnyAsync(wc => wc.Name.ToLower() == name.ToLower() && (excludeId == null || wc.Id != excludeId));
    }
}
