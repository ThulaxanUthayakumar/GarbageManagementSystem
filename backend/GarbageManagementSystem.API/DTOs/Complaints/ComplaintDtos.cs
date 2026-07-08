using GarbageManagementSystem.API.Models.Enums;

namespace GarbageManagementSystem.API.DTOs.Complaints;

public class ComplaintDto
{
    public int Id { get; set; }
    public int ResidentId { get; set; }
    public string ResidentName { get; set; } = string.Empty;
    public string Zone { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public ComplaintStatus Status { get; set; }
    public string? AdminRemarks { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? ResolvedDate { get; set; }
}

/// <summary>Bound from multipart/form-data so the resident can attach a supporting photo.</summary>
public class CreateComplaintDto
{
    public string Subject { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Microsoft.AspNetCore.Http.IFormFile? Image { get; set; }
}

public class UpdateComplaintStatusDto
{
    public ComplaintStatus Status { get; set; }
    public string? AdminRemarks { get; set; }
}
