using GMS.Domain.Common;

namespace GMS.Domain.Entities;

/// <summary>
/// Resident satisfaction feedback: either tied to a resolved complaint
/// (one per complaint, unique filtered index in Phase 2) or general service
/// feedback when ComplaintId is null. Rating is 1-5 (CHECK constraint in Phase 2).
/// </summary>
public class Feedback : BaseEntity
{
    public int ResidentId { get; set; }     // FK -> Residents
    public int? ComplaintId { get; set; }   // FK -> Complaints (nullable)

    public int Rating { get; set; }         // 1..5
    public string? Comments { get; set; }

    // Navigation
    public Resident Resident { get; set; } = null!;
    public Complaint? Complaint { get; set; }
}
