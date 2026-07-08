using System.Security.Claims;
using GarbageManagementSystem.API.Common;
using GarbageManagementSystem.API.DTOs.Common;
using GarbageManagementSystem.API.DTOs.Schedules;
using GarbageManagementSystem.API.Models.Enums;
using GarbageManagementSystem.API.Repositories;
using GarbageManagementSystem.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GarbageManagementSystem.API.Controllers;

/// <summary>Collection schedules: admins plan them per zone; residents view their own zone's schedule.</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SchedulesController : ControllerBase
{
    private readonly IScheduleService _scheduleService;
    private readonly IResidentRepository _residentRepository;

    public SchedulesController(IScheduleService scheduleService, IResidentRepository residentRepository)
    {
        _scheduleService = scheduleService;
        _residentRepository = residentRepository;
    }

    /// <summary>Admin: paged list of every schedule entry, optionally filtered by zone.</summary>
    [HttpGet]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<ActionResult<ApiResponse<PagedResultDto<ScheduleDto>>>> GetAll([FromQuery] PaginationParams pagination, [FromQuery] string? zone)
    {
        var result = await _scheduleService.GetPagedAsync(pagination, zone);
        return Ok(ApiResponse<PagedResultDto<ScheduleDto>>.SuccessResponse(result));
    }

    /// <summary>Resident: the upcoming schedule for their own zone.</summary>
    [HttpGet("my")]
    [Authorize(Roles = UserRoles.Resident)]
    public async Task<ActionResult<ApiResponse<List<ScheduleDto>>>> GetMy()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var resident = await _residentRepository.GetByApplicationUserIdAsync(userId);

        if (resident is null)
        {
            throw new NotFoundException("No resident profile is linked to this account.");
        }

        var result = await _scheduleService.GetByZoneAsync(resident.Zone);
        return Ok(ApiResponse<List<ScheduleDto>>.SuccessResponse(result));
    }

    /// <summary>Returns a single schedule entry.</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<ScheduleDto>>> GetById(int id)
    {
        var result = await _scheduleService.GetByIdAsync(id);
        return Ok(ApiResponse<ScheduleDto>.SuccessResponse(result));
    }

    /// <summary>Admin: creates a new schedule entry for a zone.</summary>
    [HttpPost]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<ActionResult<ApiResponse<ScheduleDto>>> Create([FromBody] CreateScheduleDto dto)
    {
        var result = await _scheduleService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<ScheduleDto>.SuccessResponse(result, "Schedule created."));
    }

    /// <summary>Admin: updates a schedule entry.</summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<ActionResult<ApiResponse<ScheduleDto>>> Update(int id, [FromBody] UpdateScheduleDto dto)
    {
        var result = await _scheduleService.UpdateAsync(id, dto);
        return Ok(ApiResponse<ScheduleDto>.SuccessResponse(result, "Schedule updated."));
    }

    /// <summary>Admin: deletes a schedule entry.</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
    {
        await _scheduleService.DeleteAsync(id);
        return Ok(ApiResponse<object>.SuccessResponse(new { }, "Schedule deleted."));
    }
}
