using GMS.Domain.Common;
using GMS.Domain.Identity;

namespace GMS.Domain.Entities;

/// <summary>
/// Role profile for a municipal administrator (1:1 with ApplicationUser),
/// scoped to a single municipality.
/// </summary>
public class MunicipalAdmin : BaseEntity
{
    public int UserId { get; set; }               // FK -> AspNetUsers (unique)
    public int MunicipalityId { get; set; }       // FK -> Municipalities

    public string? Designation { get; set; }

    // Navigation
    public ApplicationUser User { get; set; } = null!;
    public Municipality Municipality { get; set; } = null!;
    public ICollection<Complaint> ApprovedComplaints { get; set; } = new List<Complaint>();
}
