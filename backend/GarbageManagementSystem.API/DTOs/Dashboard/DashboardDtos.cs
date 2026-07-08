namespace GarbageManagementSystem.API.DTOs.Dashboard;

/// <summary>Top-line counters shown as cards on the admin dashboard.</summary>
public class DashboardStatsDto
{
    public int TotalResidents { get; set; }
    public int ActiveResidents { get; set; }
    public int PendingRequests { get; set; }
    public int InProgressRequests { get; set; }
    public int CompletedCollections { get; set; }
    public int TotalComplaints { get; set; }
    public int OpenComplaints { get; set; }
    public int UpcomingSchedules { get; set; }
}

/// <summary>Quick personal counters shown on a resident's own dashboard.</summary>
public class ResidentDashboardSummaryDto
{
    public int MyTotalRequests { get; set; }
    public int MyPendingRequests { get; set; }
    public int MyCompletedRequests { get; set; }
    public int MyOpenComplaints { get; set; }
    public int MyTotalComplaints { get; set; }
    public string Zone { get; set; } = string.Empty;
}

/// <summary>One point on a month-by-month trend chart.</summary>
public class MonthlyTrendPointDto
{
    public string Month { get; set; } = string.Empty;
    public int Completed { get; set; }
    public int Pending { get; set; }
    public int Cancelled { get; set; }
}

/// <summary>One slice of a category breakdown pie/bar chart.</summary>
public class CategoryCountDto
{
    public string Category { get; set; } = string.Empty;
    public int Count { get; set; }
}
