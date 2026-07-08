using GarbageManagementSystem.API.DTOs.Common;
using GarbageManagementSystem.API.DTOs.Residents;
using GarbageManagementSystem.API.Models.Enums;
using GarbageManagementSystem.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GarbageManagementSystem.API.Controllers;

/// <summary>Admin-only management of resident profiles.</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = UserRoles.Admin)]
public class ResidentsController : ControllerBase
{
    private readonly IResidentService _residentService;

    public ResidentsController(IResidentService residentService)
    {
        _residentService = residentService;
    }

    /// <summary>Returns a paged, searchable list of residents.</summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResultDto<ResidentDto>>>> GetAll([FromQuery] PaginationParams pagination)
    {
        var result = await _residentService.GetPagedAsync(pagination);
        return Ok(ApiResponse<PagedResultDto<ResidentDto>>.SuccessResponse(result));
    }

    /// <summary>Returns full details for a single resident.</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<ResidentDto>>> GetById(int id)
    {
        var result = await _residentService.GetByIdAsync(id);
        return Ok(ApiResponse<ResidentDto>.SuccessResponse(result));
    }

    /// <summary>Creates a new resident, including their login account.</summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<ResidentDto>>> Create([FromBody] CreateResidentDto dto)
    {
        var result = await _residentService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<ResidentDto>.SuccessResponse(result, "Resident created successfully."));
    }

    /// <summary>Updates an existing resident's profile.</summary>
    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<ResidentDto>>> Update(int id, [FromBody] UpdateResidentDto dto)
    {
        var result = await _residentService.UpdateAsync(id, dto);
        return Ok(ApiResponse<ResidentDto>.SuccessResponse(result, "Resident updated successfully."));
    }

    /// <summary>Deletes a resident and their login account.</summary>
    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
    {
        await _residentService.DeleteAsync(id);
        return Ok(ApiResponse<object>.SuccessResponse(new { }, "Resident deleted successfully."));
    }
}
