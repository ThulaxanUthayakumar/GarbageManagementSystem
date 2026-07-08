using GarbageManagementSystem.API.DTOs.Common;
using GarbageManagementSystem.API.DTOs.WasteCategories;
using GarbageManagementSystem.API.Models.Enums;
using GarbageManagementSystem.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GarbageManagementSystem.API.Controllers;

/// <summary>Waste category lookup list (both roles can read; only Admin can manage).</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WasteCategoriesController : ControllerBase
{
    private readonly IWasteCategoryService _service;

    public WasteCategoriesController(IWasteCategoryService service)
    {
        _service = service;
    }

    /// <summary>Returns every waste category, used to populate dropdowns.</summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<WasteCategoryDto>>>> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(ApiResponse<List<WasteCategoryDto>>.SuccessResponse(result));
    }

    /// <summary>Returns a single waste category.</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<WasteCategoryDto>>> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return Ok(ApiResponse<WasteCategoryDto>.SuccessResponse(result));
    }

    /// <summary>Creates a new waste category.</summary>
    [HttpPost]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<ActionResult<ApiResponse<WasteCategoryDto>>> Create([FromBody] CreateWasteCategoryDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<WasteCategoryDto>.SuccessResponse(result, "Waste category created."));
    }

    /// <summary>Updates a waste category.</summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<ActionResult<ApiResponse<WasteCategoryDto>>> Update(int id, [FromBody] UpdateWasteCategoryDto dto)
    {
        var result = await _service.UpdateAsync(id, dto);
        return Ok(ApiResponse<WasteCategoryDto>.SuccessResponse(result, "Waste category updated."));
    }

    /// <summary>Deletes a waste category.</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return Ok(ApiResponse<object>.SuccessResponse(new { }, "Waste category deleted."));
    }
}
