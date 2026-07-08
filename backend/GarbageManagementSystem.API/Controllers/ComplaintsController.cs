using System.Security.Claims;
using GarbageManagementSystem.API.Common;
using GarbageManagementSystem.API.DTOs.Common;
using GarbageManagementSystem.API.DTOs.Complaints;
using GarbageManagementSystem.API.Models.Enums;
using GarbageManagementSystem.API.Repositories;
using GarbageManagementSystem.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GarbageManagementSystem.API.Controllers;

/// <summary>Resident complaints: residents submit and track their own; admins triage all.</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ComplaintsController : ControllerBase
{
    private readonly IComplaintService _complaintService;
    private readonly IResidentRepository _residentRepository;

    public ComplaintsController(IComplaintService complaintService, IResidentRepository residentRepository)
    {
        _complaintService = complaintService;
        _residentRepository = residentRepository;
    }

    /// <summary>Admin: paged, filterable list of every complaint.</summary>
    [HttpGet]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<ActionResult<ApiResponse<PagedResultDto<ComplaintDto>>>> GetAll(
        [FromQuery] PaginationParams pagination, [FromQuery] ComplaintStatus? status)
    {
        var result = await _complaintService.GetPagedAsync(pagination, status);
        return Ok(ApiResponse<PagedResultDto<ComplaintDto>>.SuccessResponse(result));
    }

    /// <summary>Resident: the complaints they have personally submitted.</summary>
    [HttpGet("my")]
    [Authorize(Roles = UserRoles.Resident)]
    public async Task<ActionResult<ApiResponse<List<ComplaintDto>>>> GetMy()
    {
        var residentId = await ResolveResidentIdAsync();
        var result = await _complaintService.GetMyComplaintsAsync(residentId);
        return Ok(ApiResponse<List<ComplaintDto>>.SuccessResponse(result));
    }

    /// <summary>Returns a single complaint. Admins can view any; residents only their own.</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<ComplaintDto>>> GetById(int id)
    {
        var result = await _complaintService.GetByIdAsync(id);

        if (User.IsInRole(UserRoles.Resident))
        {
            var residentId = await ResolveResidentIdAsync();
            if (result.ResidentId != residentId)
            {
                throw new ForbiddenException("You can only view your own complaints.");
            }
        }

        return Ok(ApiResponse<ComplaintDto>.SuccessResponse(result));
    }

    /// <summary>Resident: submits a new complaint, optionally with a supporting photo.</summary>
    [HttpPost]
    [Authorize(Roles = UserRoles.Resident)]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<ApiResponse<ComplaintDto>>> Create([FromForm] CreateComplaintDto dto)
    {
        var residentId = await ResolveResidentIdAsync();
        var result = await _complaintService.CreateAsync(residentId, dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<ComplaintDto>.SuccessResponse(result, "Complaint submitted."));
    }

    /// <summary>Admin: updates the status and remarks on a complaint.</summary>
    [HttpPut("{id:int}/status")]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<ActionResult<ApiResponse<ComplaintDto>>> UpdateStatus(int id, [FromBody] UpdateComplaintStatusDto dto)
    {
        var result = await _complaintService.UpdateStatusAsync(id, dto);
        return Ok(ApiResponse<ComplaintDto>.SuccessResponse(result, "Complaint status updated."));
    }

    /// <summary>Admin: deletes a complaint.</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
    {
        await _complaintService.DeleteAsync(id);
        return Ok(ApiResponse<object>.SuccessResponse(new { }, "Complaint deleted."));
    }

    private async Task<int> ResolveResidentIdAsync()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var resident = await _residentRepository.GetByApplicationUserIdAsync(userId);

        if (resident is null)
        {
            throw new NotFoundException("No resident profile is linked to this account.");
        }

        return resident.Id;
    }
}
