using GMS.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace GMS.Domain.Identity;

/// <summary>
/// Application user built on ASP.NET Identity with an int primary key.
/// Every person in the system (SuperAdmin, MunicipalAdmin, GarbageCollector,
/// Resident) is an ApplicationUser; role-specific data lives in the
/// Resident / Collector / MunicipalAdmin profile entities (1:1).
/// </summary>
public class ApplicationUser : IdentityUser<int>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? ProfileImageUrl { get; set; }
    public bool IsActive { get; set; } = true;

    // Audit + soft delete (Identity users don't inherit BaseEntity,
    // so the fields are declared explicitly and handled by the same interceptor).
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAtUtc { get; set; }

    public string FullName => $"{FirstName} {LastName}".Trim();

    // 1:1 role profiles (at most one is populated per user)
    public Resident? Resident { get; set; }
    public Collector? Collector { get; set; }
    public MunicipalAdmin? MunicipalAdmin { get; set; }

    // 1:N
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
