using GMS.Domain.Common;

namespace GMS.Domain.Entities;

/// <summary>
/// Waste classification (Plastic, Paper, Glass, Metal, Organic, Electronic
/// Waste, Hazardous Waste). Kept as a table rather than an enum so admins can
/// add categories without a deployment; seeded in Phase 2.
/// </summary>
public class WasteCategory : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool RequiresSpecialHandling { get; set; } // e.g. Hazardous, E-Waste
    public bool IsActive { get; set; } = true;

    // Navigation
    public ICollection<Complaint> Complaints { get; set; } = new List<Complaint>();
    public ICollection<CollectionSchedule> Schedules { get; set; } = new List<CollectionSchedule>();
    public ICollection<CollectionRecord> CollectionRecords { get; set; } = new List<CollectionRecord>();
}
