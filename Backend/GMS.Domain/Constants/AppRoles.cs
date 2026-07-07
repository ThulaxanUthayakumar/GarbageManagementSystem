namespace GMS.Domain.Constants;

/// <summary>
/// Canonical role names used across Identity seeding, [Authorize] policies
/// and the frontend. Defined once to avoid magic strings.
/// </summary>
public static class AppRoles
{
    public const string SuperAdmin = "SuperAdmin";
    public const string MunicipalAdmin = "MunicipalAdmin";
    public const string GarbageCollector = "GarbageCollector";
    public const string Resident = "Resident";

    public static readonly IReadOnlyList<string> All = new[]
    {
        SuperAdmin,
        MunicipalAdmin,
        GarbageCollector,
        Resident
    };
}
