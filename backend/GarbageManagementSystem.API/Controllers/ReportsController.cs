using GarbageManagementSystem.API.DTOs.Common;
using GarbageManagementSystem.API.DTOs.Reports;
using GarbageManagementSystem.API.Models.Enums;
using GarbageManagementSystem.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GarbageManagementSystem.API.Controllers;

/// <summary>Admin-only analytical reports over collections and complaints.</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = UserRoles.Admin)]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    /// <summary>Collection totals for a single day (defaults to today).</summary>
    [HttpGet("daily-collections")]
    public async Task<ActionResult<ApiResponse<DailyCollectionReportDto>>> GetDailyCollections([FromQuery] DateTime? date)
    {
        var result = await _reportService.GetDailyCollectionReportAsync(date ?? DateTime.UtcNow.Date);
        return Ok(ApiResponse<DailyCollectionReportDto>.SuccessResponse(result));
    }

    /// <summary>Collection totals and a day-by-day breakdown for a given month.</summary>
    [HttpGet("monthly-collections")]
    public async Task<ActionResult<ApiResponse<MonthlyCollectionReportDto>>> GetMonthlyCollections([FromQuery] int? year, [FromQuery] int? month)
    {
        var now = DateTime.UtcNow;
        var result = await _reportService.GetMonthlyCollectionReportAsync(year ?? now.Year, month ?? now.Month);
        return Ok(ApiResponse<MonthlyCollectionReportDto>.SuccessResponse(result));
    }

    /// <summary>Complaint counts by status, plus average resolution time.</summary>
    [HttpGet("complaint-statistics")]
    public async Task<ActionResult<ApiResponse<ComplaintStatisticsDto>>> GetComplaintStatistics()
    {
        var result = await _reportService.GetComplaintStatisticsAsync();
        return Ok(ApiResponse<ComplaintStatisticsDto>.SuccessResponse(result));
    }

    /// <summary>Request counts grouped by waste category.</summary>
    [HttpGet("waste-category-statistics")]
    public async Task<ActionResult<ApiResponse<WasteCategoryStatisticsDto>>> GetWasteCategoryStatistics()
    {
        var result = await _reportService.GetWasteCategoryStatisticsAsync();
        return Ok(ApiResponse<WasteCategoryStatisticsDto>.SuccessResponse(result));
    }
}
