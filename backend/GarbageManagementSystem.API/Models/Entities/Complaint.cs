using GarbageManagementSystem.API.Models.Enums;

namespace GarbageManagementSystem.API.Models.Entities;

/// <summary>
/// A complaint raised by a resident (missed pickup, damaged bin, etc.), tracked to resolution.
/// </summary>
public class Complaint
{
    public int Id { get; set; }

    public int ResidentId { get; set; }
    public Resident? Resident { get; set; }

    public string Subject { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Relative path (e.g. "/uploads/complaints/xxxx.jpg") to an optional supporting photo.
    /// </summary>
    public string? ImageUrl { get; set; }

    public ComplaintStatus Status { get; set; } = ComplaintStatus.Open;
    public string? AdminRemarks { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? ResolvedDate { get; set; }
}
