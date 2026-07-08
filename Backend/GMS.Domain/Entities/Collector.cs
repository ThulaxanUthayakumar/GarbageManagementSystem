using GMS.Domain.Common;
using GMS.Domain.Identity;

namespace GMS.Domain.Entities;

/// <summary>
/// Role profile for a garbage collector (1:1 with ApplicationUser).
/// Carries operational data: employee code, vehicle, availability.
/// </summary>
public class Collector : BaseEntity
{
    public int UserId { get; set; }               // FK -> AspNetUsers (unique)
    public int MunicipalityId { get; set; }       // FK -> Municipalities

    public string EmployeeCode { get; set; } = string.Empty;
    public string? VehicleNumber { get; set; }
    public string? LicenseNumber { get; set; }
    public bool IsAvailable { get; set; } = true;
    public DateOnly JoinedOn { get; set; }

    // Navigation
    public ApplicationUser User { get; set; } = null!;
    public Municipality Municipality { get; set; } = null!;
    public ICollection<Complaint> AssignedComplaints { get; set; } = new List<Complaint>();
    public ICollection<CollectionSchedule> Schedules { get; set; } = new List<CollectionSchedule>();
    public ICollection<CollectionRecord> CollectionRecords { get; set; } = new List<CollectionRecord>();
}
