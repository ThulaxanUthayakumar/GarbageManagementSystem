using GMS.Domain.Common;

namespace GMS.Domain.Entities;

/// <summary>
/// Proof that a pickup actually happened. Created either against a scheduled
/// route run (CollectionScheduleId) or as the resolution of a complaint
/// (ComplaintId) - at least one of the two is set, enforced by a CHECK
/// constraint in Phase 2. Carries GPS coordinates and photo proof.
/// </summary>
public class CollectionRecord : BaseEntity
{
    public int? CollectionScheduleId { get; set; } // FK -> CollectionSchedules (nullable)
    public int? ComplaintId { get; set; }          // FK -> Complaints (nullable)
    public int CollectorId { get; set; }           // FK -> Collectors
    public int? WasteCategoryId { get; set; }      // FK -> WasteCategories

    public DateTime CollectedAtUtc { get; set; }
    public decimal? WeightKg { get; set; }         // decimal(10,2), configured in Phase 2

    // GPS location captured at completion time
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

    public string? Notes { get; set; }

    // Navigation
    public CollectionSchedule? CollectionSchedule { get; set; }
    public Complaint? Complaint { get; set; }
    public Collector Collector { get; set; } = null!;
    public WasteCategory? WasteCategory { get; set; }
    public ICollection<Image> Images { get; set; } = new List<Image>();
}
