using GMS.Domain.Common;
using GMS.Domain.Enums;

namespace GMS.Domain.Entities;

/// <summary>
/// A resident's report of uncollected garbage. Central aggregate of the
/// Complaint Module: carries status workflow, assignment, geo location,
/// images, comments, status history and resolution data.
/// </summary>
public class Complaint : BaseEntity
{
    /// <summary>Human-friendly tracking code, e.g. CMP-2026-000123 (generated in the service layer, unique).</summary>
    public string TrackingCode { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public int ResidentId { get; set; }             // FK -> Residents
    public int? WasteCategoryId { get; set; }       // FK -> WasteCategories

    public ComplaintStatus Status { get; set; } = ComplaintStatus.Pending;
    public ComplaintPriority Priority { get; set; } = ComplaintPriority.Medium;

    // Location of the reported garbage
    public string? Address { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

    // Workflow
    public int? AssignedCollectorId { get; set; }   // FK -> Collectors
    public DateTime? AssignedAtUtc { get; set; }
    public int? ApprovedByAdminId { get; set; }     // FK -> MunicipalAdmins
    public DateTime? ApprovedAtUtc { get; set; }
    public DateTime? ResolvedAtUtc { get; set; }
    public DateTime? ClosedAtUtc { get; set; }
    public string? ResolutionNotes { get; set; }

    // Navigation
    public Resident Resident { get; set; } = null!;
    public WasteCategory? WasteCategory { get; set; }
    public Collector? AssignedCollector { get; set; }
    public MunicipalAdmin? ApprovedByAdmin { get; set; }
    public Feedback? Feedback { get; set; }
    public ICollection<Image> Images { get; set; } = new List<Image>();
    public ICollection<ComplaintComment> Comments { get; set; } = new List<ComplaintComment>();
    public ICollection<ComplaintStatusHistory> StatusHistory { get; set; } = new List<ComplaintStatusHistory>();
    public ICollection<CollectionRecord> CollectionRecords { get; set; } = new List<CollectionRecord>();
}
