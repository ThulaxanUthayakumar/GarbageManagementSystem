using GMS.Domain.Common;

namespace GMS.Domain.Entities;

/// <summary>
/// A collection route inside a municipality: the unit collectors are
/// scheduled against and residents are mapped to.
/// </summary>
public class Route : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty; // e.g. RT-COL-004, unique per municipality
    public int MunicipalityId { get; set; }          // FK -> Municipalities

    public string? Description { get; set; }
    public string? AreaCovered { get; set; }
    public double? StartLatitude { get; set; }
    public double? StartLongitude { get; set; }
    public double? EndLatitude { get; set; }
    public double? EndLongitude { get; set; }
    public int EstimatedDurationMinutes { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation
    public Municipality Municipality { get; set; } = null!;
    public ICollection<Resident> Residents { get; set; } = new List<Resident>();
    public ICollection<CollectionSchedule> Schedules { get; set; } = new List<CollectionSchedule>();
}
