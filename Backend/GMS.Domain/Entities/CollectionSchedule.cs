using GMS.Domain.Common;
using GMS.Domain.Enums;

namespace GMS.Domain.Entities;

/// <summary>
/// A planned pickup: a collector working a route on a specific date and time
/// window, optionally for a specific waste category. Residents see these as
/// "View Collection Schedule"; collectors see them as "Daily Route".
/// EF Core 8 maps DateOnly/TimeOnly natively to SQL Server date/time columns.
/// </summary>
public class CollectionSchedule : BaseEntity
{
    public int RouteId { get; set; }            // FK -> Routes
    public int CollectorId { get; set; }        // FK -> Collectors
    public int? WasteCategoryId { get; set; }   // FK -> WasteCategories (null = mixed/general)

    public DateOnly ScheduledDate { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }

    public ScheduleStatus Status { get; set; } = ScheduleStatus.Scheduled;
    public string? Notes { get; set; }

    // Navigation
    public Route Route { get; set; } = null!;
    public Collector Collector { get; set; } = null!;
    public WasteCategory? WasteCategory { get; set; }
    public ICollection<CollectionRecord> CollectionRecords { get; set; } = new List<CollectionRecord>();
}
