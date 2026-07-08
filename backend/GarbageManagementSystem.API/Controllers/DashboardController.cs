using System.Security.Claims;
using GarbageManagementSystem.API.Common;
using GarbageManagementSystem.API.DTOs.Common;
using GarbageManagementSystem.API.DTOs.Dashboard;
using GarbageManagementSystem.API.Models.Enums;
using GarbageManagementSystem.API.Repositories;
using GarbageManagementSystem.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GarbageManagementSystem.API.Controllers;

/// <summary>Dashboard statistics and chart data, tailored to the caller's role.</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;
    private readonly IResidentRepository _residentRepository;

    public DashboardController(IDashboardService dashboardService, IResidentRepository residentRepository)
    {
        _dashboardService = dashboardService;
        _residentRepository = residentRepository;
    }

    /// <summary>Admin: top-line counters (residents, requests, complaints, schedules).</summary>
    [HttpGet("stats")]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<ActionResult<ApiResponse<DashboardStatsDto>>> GetStats()
    {
        var result = await _dashboardService.GetAdminStatsAsync();
        return Ok(ApiResponse<DashboardStatsDto>.SuccessResponse(result));
    }

    /// <summary>Resident: personal counters for their own requests and complaints.</summary>
    [HttpGet("resident-summary")]
    [Authorize(Roles = UserRoles.Resident)]
    public async Task<ActionResult<ApiResponse<ResidentDashboardSummaryDto>>> GetResidentSummary()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var resident = await _residentRepository.GetByApplicationUserIdAsync(userId);

        if (resident is null)
        {
            throw new NotFoundException("No resident profile is linked to this account.");
        }

        var result = await _dashboardService.GetResidentSummaryAsync(resident.Id);
        return Ok(ApiResponse<ResidentDashboardSummaryDto>.SuccessResponse(result));
    }

    /// <summary>Admin: monthly collection trend for the last N months (default 6).</summary>
    [HttpGet("collection-trend")]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<ActionResult<ApiResponse<List<MonthlyTrendPointDto>>>> GetCollectionTrend([FromQuery] int months = 6)
    {
        var safeMonths = months is < 1 or > 24 ? 6 : months;
        var result = await _dashboardService.GetCollectionTrendAsync(safeMonths);
        return Ok(ApiResponse<List<MonthlyTrendPointDto>>.SuccessResponse(result));
    }

    /// <summary>Admin: request counts grouped by waste category, for the dashboard pie chart.</summary>
    [HttpGet("waste-category-breakdown")]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<ActionResult<ApiResponse<List<CategoryCountDto>>>> GetWasteCategoryBreakdown()
    {
        var result = await _dashboardService.GetWasteCategoryBreakdownAsync();
        return Ok(ApiResponse<List<CategoryCountDto>>.SuccessResponse(result));
    }
}
