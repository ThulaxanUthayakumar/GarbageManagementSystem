namespace GMS.Domain.Common;

/// <summary>
/// Contract for entities that are never physically removed. The persistence layer
/// converts deletes into IsDeleted = true and applies a global query filter so
/// soft-deleted rows are excluded from all queries by default.
/// </summary>
public interface ISoftDeletable
{
    bool IsDeleted { get; set; }
    DateTime? DeletedAtUtc { get; set; }
    string? DeletedBy { get; set; }
}
