using GarbageManagementSystem.API.Models.Enums;

namespace GarbageManagementSystem.API.DTOs.CollectionRequests;

public class CollectionRequestDto
{
    public int Id { get; set; }
    public int ResidentId { get; set; }
    public string ResidentName { get; set; } = string.Empty;
    public string Zone { get; set; } = string.Empty;
    public int WasteCategoryId { get; set; }
    public string WasteCategoryName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public DateTime PickupDate { get; set; }
    public RequestStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? CompletedDate { get; set; }
}

/// <summary>Bound from multipart/form-data so the resident can attach a photo of the waste.</summary>
public class CreateCollectionRequestDto
{
    public int WasteCategoryId { get; set; }
    public string? Description { get; set; }
    public DateTime PickupDate { get; set; }
    public Microsoft.AspNetCore.Http.IFormFile? Image { get; set; }
}

public class UpdateCollectionRequestStatusDto
{
    public RequestStatus Status { get; set; }
}
