using GMS.Domain.Common;
using GMS.Domain.Enums;
using GMS.Domain.Identity;

namespace GMS.Domain.Entities;

/// <summary>
/// In-app notification for a user. Email delivery (Phase 6+) reuses the same
/// record; RelatedEntityType/RelatedEntityId let the frontend deep-link
/// (e.g. "Complaint" + 42 -> /complaints/42).
/// </summary>
public class Notification : BaseEntity
{
    public int UserId { get; set; }              // FK -> AspNetUsers (recipient)

    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; } = NotificationType.General;

    public bool IsRead { get; set; }
    public DateTime? ReadAtUtc { get; set; }

    public string? RelatedEntityType { get; set; } // e.g. "Complaint", "CollectionSchedule"
    public int? RelatedEntityId { get; set; }

    // Navigation
    public ApplicationUser User { get; set; } = null!;
}
