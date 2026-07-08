using GarbageManagementSystem.API.DTOs.Dashboard;

namespace GarbageManagementSystem.API.DTOs.Reports;

public class DailyCollectionReportDto
{
    public DateTime Date { get; set; }
    public int TotalRequests { get; set; }
    public int Completed { get; set; }
    public int Pending { get; set; }
    public int InProgress { get; set; }
    public int Cancelled { get; set; }
    public List<CategoryCountDto> ByCategory { get; set; } = new();
}

public class DailyPointDto
{
    public int Day { get; set; }
    public int Count { get; set; }
}

public class MonthlyCollectionReportDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public string MonthName { get; set; } = string.Empty;
    public int TotalRequests { get; set; }
    public int Completed { get; set; }
    public int Pending { get; set; }
    public int InProgress { get; set; }
    public int Cancelled { get; set; }
    public List<DailyPointDto> DailyBreakdown { get; set; } = new();
}

public class ComplaintStatisticsDto
{
    public int Total { get; set; }
    public int Open { get; set; }
    public int InProgress { get; set; }
    public int Resolved { get; set; }
    public int Closed { get; set; }
    public double AverageResolutionDays { get; set; }
}

public class WasteCategoryStatisticsDto
{
    public int TotalRequests { get; set; }
    public List<CategoryCountDto> Categories { get; set; } = new();
}
