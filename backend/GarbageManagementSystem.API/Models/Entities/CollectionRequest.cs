using GarbageManagementSystem.API.Models.Enums;

namespace GarbageManagementSystem.API.Models.Entities;

/// <summary>
/// A resident's request to have a specific type of waste picked up on a chosen date.
/// </summary>
public class CollectionRequest
{
    public int Id { get; set; }

    public int ResidentId { get; set; }
    public Resident? Resident { get; set; }

    public int WasteCategoryId { get; set; }
    public WasteCategory? WasteCategory { get; set; }

    public string? Description { get; set; }

    /// <summary>
    /// Relative path (e.g. "/uploads/requests/xxxx.jpg") to an optional photo of the waste.
    /// </summary>
    public string? ImageUrl { get; set; }

    public DateTime PickupDate { get; set; }
    public RequestStatus Status { get; set; } = RequestStatus.Pending;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? CompletedDate { get; set; }
}
