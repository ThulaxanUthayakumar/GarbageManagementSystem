using GarbageManagementSystem.API.Common;
using GarbageManagementSystem.API.DTOs.Auth;
using GarbageManagementSystem.API.Models.Entities;
using GarbageManagementSystem.API.Models.Enums;
using GarbageManagementSystem.API.Repositories;
using Microsoft.AspNetCore.Identity;

namespace GarbageManagementSystem.API.Services;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
    Task<AuthResponseDto> LoginAsync(LoginDto dto);
    Task ChangePasswordAsync(string userId, ChangePasswordDto dto);
    Task<UserDto> GetCurrentUserAsync(string userId);
}

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ITokenService _tokenService;
    private readonly IResidentRepository _residentRepository;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ITokenService tokenService,
        IResidentRepository residentRepository)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _tokenService = tokenService;
        _residentRepository = residentRepository;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        var existingUser = await _userManager.FindByEmailAsync(dto.Email);
        if (existingUser is not null)
        {
            throw new BadRequestException("An account with this email address already exists.");
        }

        if (!await _roleManager.RoleExistsAsync(UserRoles.Resident))
        {
            await _roleManager.CreateAsync(new IdentityRole(UserRoles.Resident));
        }

        var user = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            FullName = dto.FullName,
            PhoneNumber = dto.PhoneNumber,
            EmailConfirmed = true,
            CreatedAt = DateTime.UtcNow
        };

        var createResult = await _userManager.CreateAsync(user, dto.Password);
        if (!createResult.Succeeded)
        {
            throw new BadRequestException(string.Join(" ", createResult.Errors.Select(e => e.Description)));
        }

        await _userManager.AddToRoleAsync(user, UserRoles.Resident);

        var resident = new Resident
        {
            ApplicationUserId = user.Id,
            Address = dto.Address,
            City = dto.City,
            State = dto.State,
            ZipCode = dto.ZipCode,
            Zone = dto.Zone,
            IsActive = true,
            DateJoined = DateTime.UtcNow
        };

        await _residentRepository.AddAsync(resident);
        await _residentRepository.SaveChangesAsync();

        return BuildAuthResponse(user, UserRoles.Resident, resident.Id);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user is null)
        {
            throw new BadRequestException("Invalid email or password.");
        }

        var passwordValid = await _userManager.CheckPasswordAsync(user, dto.Password);
        if (!passwordValid)
        {
            throw new BadRequestException("Invalid email or password.");
        }

        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? UserRoles.Resident;

        int? residentId = null;
        if (role == UserRoles.Resident)
        {
            var resident = await _residentRepository.GetByApplicationUserIdAsync(user.Id);
            residentId = resident?.Id;
        }

        return BuildAuthResponse(user, role, residentId);
    }

    public async Task ChangePasswordAsync(string userId, ChangePasswordDto dto)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            throw new NotFoundException("User", userId);
        }

        var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
        if (!result.Succeeded)
        {
            throw new BadRequestException(string.Join(" ", result.Errors.Select(e => e.Description)));
        }
    }

    public async Task<UserDto> GetCurrentUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            throw new NotFoundException("User", userId);
        }

        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? UserRoles.Resident;

        int? residentId = null;
        if (role == UserRoles.Resident)
        {
            var resident = await _residentRepository.GetByApplicationUserIdAsync(user.Id);
            residentId = resident?.Id;
        }

        return new UserDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email ?? string.Empty,
            PhoneNumber = user.PhoneNumber,
            Role = role,
            ProfileImageUrl = user.ProfileImageUrl,
            ResidentId = residentId,
            CreatedAt = user.CreatedAt
        };
    }

    private AuthResponseDto BuildAuthResponse(ApplicationUser user, string role, int? residentId)
    {
        var (token, expiresAt) = _tokenService.GenerateToken(user, role);

        return new AuthResponseDto
        {
            Token = token,
            ExpiresAt = expiresAt,
            User = new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber,
                Role = role,
                ProfileImageUrl = user.ProfileImageUrl,
                ResidentId = residentId,
                CreatedAt = user.CreatedAt
            }
        };
    }
}
