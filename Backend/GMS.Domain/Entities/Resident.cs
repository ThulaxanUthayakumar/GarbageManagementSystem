using GMS.Domain.Common;
using GMS.Domain.Identity;

namespace GMS.Domain.Entities;

/// <summary>
/// Role profile for a resident (1:1 with ApplicationUser).
/// Holds address/geo data used for complaint locations and route mapping.
/// </summary>
public class Resident : BaseEntity
{
    public int UserId { get; set; }               // FK -> AspNetUsers (unique)
    public int MunicipalityId { get; set; }       // FK -> Municipalities
    public int? RouteId { get; set; }             // FK -> Routes (route that serves this household)

    public string AddressLine { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public int HouseholdSize { get; set; } = 1;

    // Navigation
    public ApplicationUser User { get; set; } = null!;
    public Municipality Municipality { get; set; } = null!;
    public Route? Route { get; set; }
    public ICollection<Complaint> Complaints { get; set; } = new List<Complaint>();
    public ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();
}
