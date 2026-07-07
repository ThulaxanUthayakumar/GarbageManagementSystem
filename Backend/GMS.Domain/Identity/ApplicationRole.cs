using Microsoft.AspNetCore.Identity;

namespace GMS.Domain.Identity;

/// <summary>
/// Application role built on ASP.NET Identity with an int primary key.
/// Seeded from <see cref="GMS.Domain.Constants.AppRoles"/> in Phase 2.
/// </summary>
public class ApplicationRole : IdentityRole<int>
{
    public ApplicationRole()
    {
    }

    public ApplicationRole(string roleName, string? description = null) : base(roleName)
    {
        Description = description;
    }

    public string? Description { get; set; }
}
