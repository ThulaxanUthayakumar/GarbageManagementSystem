namespace GMS.Domain.Common;

/// <summary>
/// Contract for entities whose create/update metadata is stamped automatically
/// by the persistence layer (EF Core SaveChanges interceptor, added in Phase 2).
/// </summary>
public interface IAuditableEntity
{
    DateTime CreatedAtUtc { get; set; }
    string? CreatedBy { get; set; }
    DateTime? UpdatedAtUtc { get; set; }
    string? UpdatedBy { get; set; }
}
