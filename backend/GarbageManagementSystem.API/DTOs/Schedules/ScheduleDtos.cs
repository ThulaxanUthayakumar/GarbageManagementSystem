using GarbageManagementSystem.API.Models.Enums;

namespace GarbageManagementSystem.API.DTOs.Schedules;

public class ScheduleDto
{
    public int Id { get; set; }
    public string Zone { get; set; } = string.Empty;
    public int? WasteCategoryId { get; set; }
    public string WasteCategoryName { get; set; } = "All Categories";
    public DateTime ScheduledDate { get; set; }
    public string ScheduledTime { get; set; } = string.Empty;
    public string? CollectorName { get; set; }
    public ScheduleStatus Status { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateScheduleDto
{
    public string Zone { get; set; } = string.Empty;
    public int? WasteCategoryId { get; set; }
    public DateTime ScheduledDate { get; set; }
    public string ScheduledTime { get; set; } = string.Empty;
    public string? CollectorName { get; set; }
    public string? Notes { get; set; }
}

public class UpdateScheduleDto
{
    public string Zone { get; set; } = string.Empty;
    public int? WasteCategoryId { get; set; }
    public DateTime ScheduledDate { get; set; }
    public string ScheduledTime { get; set; } = string.Empty;
    public string? CollectorName { get; set; }
    public ScheduleStatus Status { get; set; }
    public string? Notes { get; set; }
}
