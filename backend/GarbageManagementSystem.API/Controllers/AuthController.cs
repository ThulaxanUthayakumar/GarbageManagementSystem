using System.Security.Claims;
using GarbageManagementSystem.API.DTOs.Auth;
using GarbageManagementSystem.API.DTOs.Common;
using GarbageManagementSystem.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GarbageManagementSystem.API.Controllers;

/// <summary>Handles registration, login, logout and password management.</summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>Registers a new resident account and returns a JWT so the user is logged in immediately.</summary>
    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Register([FromBody] RegisterDto dto)
    {
        var result = await _authService.RegisterAsync(dto);
        return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(result, "Registration successful."));
    }

    /// <summary>Authenticates a user and returns a JWT.</summary>
    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Login([FromBody] LoginDto dto)
    {
        var result = await _authService.LoginAsync(dto);
        return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(result, "Login successful."));
    }

    /// <summary>
    /// JWTs are stateless, so there is no server-side session to destroy. This endpoint exists
    /// for API completeness / auditing; the client is responsible for discarding the token.
    /// </summary>
    [Authorize]
    [HttpPost("logout")]
    public ActionResult<ApiResponse<object>> Logout()
    {
        return Ok(ApiResponse<object>.SuccessResponse(new { }, "Logged out successfully."));
    }

    /// <summary>Changes the password of the currently authenticated user.</summary>
    [Authorize]
    [HttpPost("change-password")]
    public async Task<ActionResult<ApiResponse<object>>> ChangePassword([FromBody] ChangePasswordDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await _authService.ChangePasswordAsync(userId, dto);
        return Ok(ApiResponse<object>.SuccessResponse(new { }, "Password changed successfully."));
    }

    /// <summary>Returns the profile of the currently authenticated user.</summary>
    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<ApiResponse<UserDto>>> Me()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var user = await _authService.GetCurrentUserAsync(userId);
        return Ok(ApiResponse<UserDto>.SuccessResponse(user));
    }
}
