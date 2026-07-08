using GarbageManagementSystem.API.Common;
using GarbageManagementSystem.API.DTOs.Dashboard;
using GarbageManagementSystem.API.Models.Enums;
using GarbageManagementSystem.API.Repositories;

namespace GarbageManagementSystem.API.Services;

public interface IDashboardService
{
    Task<DashboardStatsDto> GetAdminStatsAsync();
    Task<ResidentDashboardSummaryDto> GetResidentSummaryAsync(int residentId);
    Task<List<MonthlyTrendPointDto>> GetCollectionTrendAsync(int months);
    Task<List<CategoryCountDto>> GetWasteCategoryBreakdownAsync();
}

public class DashboardService : IDashboardService
{
    private readonly IResidentRepository _residentRepository;
    private readonly ICollectionRequestRepository _requestRepository;
    private readonly IComplaintRepository _complaintRepository;
    private readonly IScheduleRepository _scheduleRepository;
    private readonly IWasteCategoryRepository _wasteCategoryRepository;

    public DashboardService(
        IResidentRepository residentRepository,
        ICollectionRequestRepository requestRepository,
        IComplaintRepository complaintRepository,
        IScheduleRepository scheduleRepository,
        IWasteCategoryRepository wasteCategoryRepository)
    {
        _residentRepository = residentRepository;
        _requestRepository = requestRepository;
        _complaintRepository = complaintRepository;
        _scheduleRepository = scheduleRepository;
        _wasteCategoryRepository = wasteCategoryRepository;
    }

    public async Task<DashboardStatsDto> GetAdminStatsAsync()
    {
        var residents = await _residentRepository.GetAllAsync();
        var openComplaints = await _complaintRepository.CountByStatusAsync(ComplaintStatus.Open)
            + await _complaintRepository.CountByStatusAsync(ComplaintStatus.InProgress);
        var totalComplaints = (await _complaintRepository.GetAllAsync()).Count;

        return new DashboardStatsDto
        {
            TotalResidents = residents.Count,
            ActiveResidents = residents.Count(r => r.IsActive),
            PendingRequests = await _requestRepository.CountByStatusAsync(RequestStatus.Pending),
            InProgressRequests = await _requestRepository.CountByStatusAsync(RequestStatus.InProgress),
            CompletedCollections = await _requestRepository.CountByStatusAsync(RequestStatus.Completed),
            TotalComplaints = totalComplaints,
            OpenComplaints = openComplaints,
            UpcomingSchedules = await _scheduleRepository.CountUpcomingAsync()
        };
    }

    public async Task<ResidentDashboardSummaryDto> GetResidentSummaryAsync(int residentId)
    {
        var resident = await _residentRepository.GetByIdAsync(residentId);
        var myRequests = await _requestRepository.GetByResidentIdAsync(residentId);
        var myComplaints = await _complaintRepository.GetByResidentIdAsync(residentId);

        return new ResidentDashboardSummaryDto
        {
            MyTotalRequests = myRequests.Count,
            MyPendingRequests = myRequests.Count(r => r.Status == RequestStatus.Pending || r.Status == RequestStatus.InProgress),
            MyCompletedRequests = myRequests.Count(r => r.Status == RequestStatus.Completed),
            MyTotalComplaints = myComplaints.Count,
            MyOpenComplaints = myComplaints.Count(c => c.Status == ComplaintStatus.Open || c.Status == ComplaintStatus.InProgress),
            Zone = resident?.Zone ?? string.Empty
        };
    }

    public async Task<List<MonthlyTrendPointDto>> GetCollectionTrendAsync(int months)
    {
        var requests = await _requestRepository.GetAllWithDetailsAsync();
        var points = new List<MonthlyTrendPointDto>();
        var today = DateTime.UtcNow;

        for (var i = months - 1; i >= 0; i--)
        {
            var monthDate = new DateTime(today.Year, today.Month, 1).AddMonths(-i);
            var monthRequests = requests.Where(r => r.CreatedAt.Year == monthDate.Year && r.CreatedAt.Month == monthDate.Month).ToList();

            points.Add(new MonthlyTrendPointDto
            {
                Month = monthDate.ToString("MMM yyyy"),
                Completed = monthRequests.Count(r => r.Status == RequestStatus.Completed),
                Pending = monthRequests.Count(r => r.Status == RequestStatus.Pending || r.Status == RequestStatus.InProgress),
                Cancelled = monthRequests.Count(r => r.Status == RequestStatus.Cancelled)
            });
        }

        return points;
    }

    public async Task<List<CategoryCountDto>> GetWasteCategoryBreakdownAsync()
    {
        var categories = await _wasteCategoryRepository.GetAllAsync();
        var requests = await _requestRepository.GetAllWithDetailsAsync();

        return categories
            .Select(c => new CategoryCountDto
            {
                Category = c.Name,
                Count = requests.Count(r => r.WasteCategoryId == c.Id)
            })
            .ToList();
    }
}
