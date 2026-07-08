namespace GarbageManagementSystem.API.Models.Enums;

/// <summary>
/// Lifecycle status of a resident's garbage collection request.
/// </summary>
public enum RequestStatus
{
    Pending = 0,
    InProgress = 1,
    Completed = 2,
    Cancelled = 3
}
