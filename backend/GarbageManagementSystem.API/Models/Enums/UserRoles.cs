namespace GarbageManagementSystem.API.Models.Enums;

/// <summary>
/// Constant role names used with ASP.NET Identity. Using constants (instead of
/// hard-coded strings scattered around the code) avoids typos in [Authorize(Roles = "...")].
/// </summary>
public static class UserRoles
{
    public const string Admin = "Admin";
    public const string Resident = "Resident";

    public static readonly string[] All = { Admin, Resident };
}
