namespace GMS.Domain.Common;

/// <summary>
/// Base class for all domain entities: integer surrogate key,
/// full audit trail and soft-delete support.
/// </summary>
public abstract class BaseEntity : IAuditableEntity, ISoftDeletable
{
    public int Id { get; set; }

    // Audit
    public DateTime CreatedAtUtc { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public string? UpdatedBy { get; set; }

    // Soft delete
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAtUtc { get; set; }
    public string? DeletedBy { get; set; }
}
