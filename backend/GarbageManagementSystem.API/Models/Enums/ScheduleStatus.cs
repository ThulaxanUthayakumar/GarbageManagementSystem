namespace GarbageManagementSystem.API.Models.Enums;

/// <summary>
/// Status of a planned collection schedule entry for a zone.
/// </summary>
public enum ScheduleStatus
{
    Scheduled = 0,
    Completed = 1,
    Missed = 2,
    Cancelled = 3
}
