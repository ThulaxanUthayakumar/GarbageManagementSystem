using GarbageManagementSystem.API.DTOs.Dashboard;
using GarbageManagementSystem.API.DTOs.Reports;
using GarbageManagementSystem.API.Models.Enums;
using GarbageManagementSystem.API.Repositories;
using System.Globalization;

namespace GarbageManagementSystem.API.Services;

public interface IReportService
{
    Task<DailyCollectionReportDto> GetDailyCollectionReportAsync(DateTime date);
    Task<MonthlyCollectionReportDto> GetMonthlyCollectionReportAsync(int year, int month);
    Task<ComplaintStatisticsDto> GetComplaintStatisticsAsync();
    Task<WasteCategoryStatisticsDto> GetWasteCategoryStatisticsAsync();
}

public class ReportService : IReportService
{
    private readonly ICollectionRequestRepository _requestRepository;
    private readonly IComplaintRepository _complaintRepository;
    private readonly IWasteCategoryRepository _wasteCategoryRepository;

    public ReportService(
        ICollectionRequestRepository requestRepository,
        IComplaintRepository complaintRepository,
        IWasteCategoryRepository wasteCategoryRepository)
    {
        _requestRepository = requestRepository;
        _complaintRepository = complaintRepository;
        _wasteCategoryRepository = wasteCategoryRepository;
    }

    public async Task<DailyCollectionReportDto> GetDailyCollectionReportAsync(DateTime date)
    {
        var from = date.Date;
        var to = from.AddDays(1);
        var requests = await _requestRepository.GetForDateRangeAsync(from, to);
        var categories = await _wasteCategoryRepository.GetAllAsync();

        return new DailyCollectionReportDto
        {
            Date = from,
            TotalRequests = requests.Count,
            Completed = requests.Count(r => r.Status == RequestStatus.Completed),
            Pending = requests.Count(r => r.Status == RequestStatus.Pending),
            InProgress = requests.Count(r => r.Status == RequestStatus.InProgress),
            Cancelled = requests.Count(r => r.Status == RequestStatus.Cancelled),
            ByCategory = categories
                .Select(c => new CategoryCountDto { Category = c.Name, Count = requests.Count(r => r.WasteCategoryId == c.Id) })
                .Where(c => c.Count > 0)
                .ToList()
        };
    }

    public async Task<MonthlyCollectionReportDto> GetMonthlyCollectionReportAsync(int year, int month)
    {
        var from = new DateTime(year, month, 1);
        var to = from.AddMonths(1);
        var requests = await _requestRepository.GetForDateRangeAsync(from, to);
        var daysInMonth = DateTime.DaysInMonth(year, month);

        var dailyBreakdown = Enumerable.Range(1, daysInMonth)
            .Select(day => new DailyPointDto { Day = day, Count = requests.Count(r => r.CreatedAt.Day == day) })
            .ToList();

        return new MonthlyCollectionReportDto
        {
            Year = year,
            Month = month,
            MonthName = from.ToString("MMMM", CultureInfo.InvariantCulture),
            TotalRequests = requests.Count,
            Completed = requests.Count(r => r.Status == RequestStatus.Completed),
            Pending = requests.Count(r => r.Status == RequestStatus.Pending),
            InProgress = requests.Count(r => r.Status == RequestStatus.InProgress),
            Cancelled = requests.Count(r => r.Status == RequestStatus.Cancelled),
            DailyBreakdown = dailyBreakdown
        };
    }

    public async Task<ComplaintStatisticsDto> GetComplaintStatisticsAsync()
    {
        var complaints = await _complaintRepository.GetAllAsync();
        var resolved = complaints.Where(c => c.ResolvedDate.HasValue).ToList();

        var averageResolutionDays = resolved.Count > 0
            ? resolved.Average(c => (c.ResolvedDate!.Value - c.CreatedAt).TotalDays)
            : 0;

        return new ComplaintStatisticsDto
        {
            Total = complaints.Count,
            Open = complaints.Count(c => c.Status == ComplaintStatus.Open),
            InProgress = complaints.Count(c => c.Status == ComplaintStatus.InProgress),
            Resolved = complaints.Count(c => c.Status == ComplaintStatus.Resolved),
            Closed = complaints.Count(c => c.Status == ComplaintStatus.Closed),
            AverageResolutionDays = Math.Round(averageResolutionDays, 1)
        };
    }

    public async Task<WasteCategoryStatisticsDto> GetWasteCategoryStatisticsAsync()
    {
        var categories = await _wasteCategoryRepository.GetAllAsync();
        var requests = await _requestRepository.GetAllWithDetailsAsync();

        return new WasteCategoryStatisticsDto
        {
            TotalRequests = requests.Count,
            Categories = categories
                .Select(c => new CategoryCountDto { Category = c.Name, Count = requests.Count(r => r.WasteCategoryId == c.Id) })
                .OrderByDescending(c => c.Count)
                .ToList()
        };
    }
}
