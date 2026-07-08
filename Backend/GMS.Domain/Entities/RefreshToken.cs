using GMS.Domain.Common;
using GMS.Domain.Identity;

namespace GMS.Domain.Entities;

/// <summary>
/// Rotating refresh token for JWT authentication (Phase 2). Old tokens are
/// revoked and chained via ReplacedByToken so token theft is detectable.
/// </summary>
public class RefreshToken : BaseEntity
{
    public string Token { get; set; } = string.Empty; // cryptographically random, unique
    public int UserId { get; set; }                   // FK -> AspNetUsers

    public DateTime ExpiresAtUtc { get; set; }
    public string? CreatedByIp { get; set; }

    public DateTime? RevokedAtUtc { get; set; }
    public string? RevokedByIp { get; set; }
    public string? ReplacedByToken { get; set; }

    // Computed helpers (not mapped to columns)
    public bool IsExpired => DateTime.UtcNow >= ExpiresAtUtc;
    public bool IsActive => RevokedAtUtc is null && !IsExpired;

    // Navigation
    public ApplicationUser User { get; set; } = null!;
}
