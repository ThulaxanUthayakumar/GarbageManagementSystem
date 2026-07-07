using GMS.Domain.Common;
using GMS.Domain.Identity;

namespace GMS.Domain.Entities;

/// <summary>
/// Threaded discussion on a complaint (resident, collector or admin).
/// IsInternal lets staff leave notes hidden from the resident.
/// </summary>
public class ComplaintComment : BaseEntity
{
    public int ComplaintId { get; set; }     // FK -> Complaints
    public int AuthorUserId { get; set; }    // FK -> AspNetUsers

    public string Content { get; set; } = string.Empty;
    public bool IsInternal { get; set; }

    // Navigation
    public Complaint Complaint { get; set; } = null!;
    public ApplicationUser AuthorUser { get; set; } = null!;
}
