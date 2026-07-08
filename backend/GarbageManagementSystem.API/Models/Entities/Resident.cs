namespace GarbageManagementSystem.API.Models.Entities;

/// <summary>
/// Domain profile for a resident, one-to-one with an ApplicationUser (Identity) account.
/// Kept separate from ApplicationUser so that Identity concerns (login, password) stay
/// decoupled from address/zone/collection-domain concerns.
/// </summary>
public class Resident
{
    public int Id { get; set; }

    public string ApplicationUserId { get; set; } = string.Empty;
    public ApplicationUser? ApplicationUser { get; set; }

    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;

    /// <summary>
    /// Collection zone/ward used to match a resident to their CollectionSchedule.
    /// </summary>
    public string Zone { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
    public DateTime DateJoined { get; set; } = DateTime.UtcNow;

    public ICollection<CollectionRequest> CollectionRequests { get; set; } = new List<CollectionRequest>();
    public ICollection<Complaint> Complaints { get; set; } = new List<Complaint>();
}
