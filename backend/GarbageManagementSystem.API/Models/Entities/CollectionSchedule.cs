using GarbageManagementSystem.API.Models.Enums;

namespace GarbageManagementSystem.API.Models.Entities;

/// <summary>
/// A planned collection run for a zone, optionally scoped to a single waste category.
/// Residents match against this by their own Zone.
/// </summary>
public class CollectionSchedule
{
    public int Id { get; set; }

    public string Zone { get; set; } = string.Empty;

    /// <summary>
    /// Null means "general collection" covering all waste categories for the zone.
    /// </summary>
    public int? WasteCategoryId { get; set; }
    public WasteCategory? WasteCategory { get; set; }

    public DateTime ScheduledDate { get; set; }
    public string ScheduledTime { get; set; } = string.Empty;
    public string? CollectorName { get; set; }
    public ScheduleStatus Status { get; set; } = ScheduleStatus.Scheduled;
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
