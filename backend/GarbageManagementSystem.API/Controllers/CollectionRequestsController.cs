using System.Security.Claims;
using GarbageManagementSystem.API.Common;
using GarbageManagementSystem.API.DTOs.Common;
using GarbageManagementSystem.API.DTOs.CollectionRequests;
using GarbageManagementSystem.API.Models.Enums;
using GarbageManagementSystem.API.Repositories;
using GarbageManagementSystem.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GarbageManagementSystem.API.Controllers;

/// <summary>Garbage collection requests: residents create and track their own; admins manage all.</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CollectionRequestsController : ControllerBase
{
    private readonly ICollectionRequestService _requestService;
    private readonly IResidentRepository _residentRepository;

    public CollectionRequestsController(ICollectionRequestService requestService, IResidentRepository residentRepository)
    {
        _requestService = requestService;
        _residentRepository = residentRepository;
    }

    /// <summary>Admin: paged, filterable list of every collection request.</summary>
    [HttpGet]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<ActionResult<ApiResponse<PagedResultDto<CollectionRequestDto>>>> GetAll(
        [FromQuery] PaginationParams pagination, [FromQuery] RequestStatus? status)
    {
        var result = await _requestService.GetPagedAsync(pagination, status);
        return Ok(ApiResponse<PagedResultDto<CollectionRequestDto>>.SuccessResponse(result));
    }

    /// <summary>Resident: the requests they have personally submitted.</summary>
    [HttpGet("my")]
    [Authorize(Roles = UserRoles.Resident)]
    public async Task<ActionResult<ApiResponse<List<CollectionRequestDto>>>> GetMy()
    {
        var residentId = await ResolveResidentIdAsync();
        var result = await _requestService.GetMyRequestsAsync(residentId);
        return Ok(ApiResponse<List<CollectionRequestDto>>.SuccessResponse(result));
    }

    /// <summary>Returns a single request. Admins can view any; residents only their own.</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<CollectionRequestDto>>> GetById(int id)
    {
        var result = await _requestService.GetByIdAsync(id);

        if (User.IsInRole(UserRoles.Resident))
        {
            var residentId = await ResolveResidentIdAsync();
            if (result.ResidentId != residentId)
            {
                throw new ForbiddenException("You can only view your own collection requests.");
            }
        }

        return Ok(ApiResponse<CollectionRequestDto>.SuccessResponse(result));
    }

    /// <summary>Resident: submits a new pickup request, optionally with a photo of the waste.</summary>
    [HttpPost]
    [Authorize(Roles = UserRoles.Resident)]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<ApiResponse<CollectionRequestDto>>> Create([FromForm] CreateCollectionRequestDto dto)
    {
        var residentId = await ResolveResidentIdAsync();
        var result = await _requestService.CreateAsync(residentId, dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<CollectionRequestDto>.SuccessResponse(result, "Collection request submitted."));
    }

    /// <summary>Admin: updates the status of a request (Pending -&gt; InProgress -&gt; Completed, etc.).</summary>
    [HttpPut("{id:int}/status")]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<ActionResult<ApiResponse<CollectionRequestDto>>> UpdateStatus(int id, [FromBody] UpdateCollectionRequestStatusDto dto)
    {
        var result = await _requestService.UpdateStatusAsync(id, dto);
        return Ok(ApiResponse<CollectionRequestDto>.SuccessResponse(result, "Request status updated."));
    }

    /// <summary>Admin: deletes a collection request.</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
    {
        await _requestService.DeleteAsync(id);
        return Ok(ApiResponse<object>.SuccessResponse(new { }, "Collection request deleted."));
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
