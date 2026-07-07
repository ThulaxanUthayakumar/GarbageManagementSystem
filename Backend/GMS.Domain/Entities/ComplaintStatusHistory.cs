using GMS.Domain.Common;
using GMS.Domain.Enums;
using GMS.Domain.Identity;

namespace GMS.Domain.Entities;

/// <summary>
/// Append-only log of complaint status transitions. Powers the resident's
/// "Track Complaint Status" timeline and admin auditability.
/// </summary>
public class ComplaintStatusHistory : BaseEntity
{
    public int ComplaintId { get; set; }          // FK -> Complaints
    public ComplaintStatus? FromStatus { get; set; } // null on creation
    public ComplaintStatus ToStatus { get; set; }
    public int ChangedByUserId { get; set; }      // FK -> AspNetUsers
    public string? Remarks { get; set; }

    // Navigation
    public Complaint Complaint { get; set; } = null!;
    public ApplicationUser ChangedByUser { get; set; } = null!;
}
