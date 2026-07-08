namespace GarbageManagementSystem.API.Models.Enums;

/// <summary>
/// Status of a resident complaint as it is triaged by the admin team.
/// </summary>
public enum ComplaintStatus
{
    Open = 0,
    InProgress = 1,
    Resolved = 2,
    Closed = 3
}
