namespace GarbageManagementSystem.API.Models.Entities;

/// <summary>
/// A category of waste (Plastic, Paper, Glass, Organic, Metal, Electronic Waste).
/// </summary>
public class WasteCategory
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    /// <summary>
    /// Name of a Material Symbols / MUI icon key used purely for frontend display.
    /// </summary>
    public string? IconName { get; set; }

    public ICollection<CollectionRequest> CollectionRequests { get; set; } = new List<CollectionRequest>();
    public ICollection<CollectionSchedule> CollectionSchedules { get; set; } = new List<CollectionSchedule>();
}
