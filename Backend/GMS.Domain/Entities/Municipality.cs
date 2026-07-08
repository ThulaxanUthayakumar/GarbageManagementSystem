using GMS.Domain.Common;

namespace GMS.Domain.Entities;

/// <summary>
/// A municipality (city corporation / council) that owns routes, staff and
/// residents. Gives SuperAdmin -> MunicipalAdmin management a real anchor and
/// keeps the system multi-tenant-ready.
/// </summary>
public class Municipality : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string? PostalCode { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation
    public ICollection<MunicipalAdmin> Admins { get; set; } = new List<MunicipalAdmin>();
    public ICollection<Resident> Residents { get; set; } = new List<Resident>();
    public ICollection<Collector> Collectors { get; set; } = new List<Collector>();
    public ICollection<Route> Routes { get; set; } = new List<Route>();
}
