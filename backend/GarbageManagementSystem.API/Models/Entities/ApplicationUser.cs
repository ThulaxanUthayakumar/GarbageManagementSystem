using Microsoft.AspNetCore.Identity;

namespace GarbageManagementSystem.API.Models.Entities;

/// <summary>
/// Extends the built-in ASP.NET Identity user with the extra profile fields
/// this application needs (full name, avatar, join date).
/// Email, PasswordHash, PhoneNumber etc. are already provided by IdentityUser.
/// </summary>
public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public string? ProfileImageUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Navigation back to the resident profile, populated only for users in the Resident role.
    /// Null for Admin users.
    /// </summary>
    public Resident? Resident { get; set; }
}
