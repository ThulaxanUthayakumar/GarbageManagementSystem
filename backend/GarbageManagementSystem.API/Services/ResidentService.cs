using AutoMapper;
using GarbageManagementSystem.API.Common;
using GarbageManagementSystem.API.DTOs.Common;
using GarbageManagementSystem.API.DTOs.Residents;
using GarbageManagementSystem.API.Models.Entities;
using GarbageManagementSystem.API.Models.Enums;
using GarbageManagementSystem.API.Repositories;
using Microsoft.AspNetCore.Identity;

namespace GarbageManagementSystem.API.Services;

public interface IResidentService
{
    Task<PagedResultDto<ResidentDto>> GetPagedAsync(PaginationParams pagination);
    Task<ResidentDto> GetByIdAsync(int id);
    Task<ResidentDto> CreateAsync(CreateResidentDto dto);
    Task<ResidentDto> UpdateAsync(int id, UpdateResidentDto dto);
    Task DeleteAsync(int id);
}

public class ResidentService : IResidentService
{
    private readonly IResidentRepository _residentRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IMapper _mapper;

    public ResidentService(
        IResidentRepository residentRepository,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IMapper mapper)
    {
        _residentRepository = residentRepository;
        _userManager = userManager;
        _roleManager = roleManager;
        _mapper = mapper;
    }

    public async Task<PagedResultDto<ResidentDto>> GetPagedAsync(PaginationParams pagination)
    {
        var (items, totalCount) = await _residentRepository.GetPagedAsync(pagination.PageNumber, pagination.PageSize, pagination.SearchTerm);

        return new PagedResultDto<ResidentDto>
        {
            Items = _mapper.Map<List<ResidentDto>>(items),
            TotalCount = totalCount,
            PageNumber = pagination.PageNumber,
            PageSize = pagination.PageSize
        };
    }

    public async Task<ResidentDto> GetByIdAsync(int id)
    {
        var resident = await _residentRepository.GetByIdWithUserAsync(id);
        if (resident is null)
        {
            throw new NotFoundException("Resident", id);
        }

        return _mapper.Map<ResidentDto>(resident);
    }

    public async Task<ResidentDto> CreateAsync(CreateResidentDto dto)
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
            DateJoined = DateTime.UtcNow,
            ApplicationUser = user
        };

        await _residentRepository.AddAsync(resident);
        await _residentRepository.SaveChangesAsync();

        return _mapper.Map<ResidentDto>(resident);
    }

    public async Task<ResidentDto> UpdateAsync(int id, UpdateResidentDto dto)
    {
        var resident = await _residentRepository.GetByIdWithUserAsync(id);
        if (resident is null)
        {
            throw new NotFoundException("Resident", id);
        }

        resident.Address = dto.Address;
        resident.City = dto.City;
        resident.State = dto.State;
        resident.ZipCode = dto.ZipCode;
        resident.Zone = dto.Zone;
        resident.IsActive = dto.IsActive;

        if (resident.ApplicationUser is not null)
        {
            resident.ApplicationUser.FullName = dto.FullName;
            resident.ApplicationUser.PhoneNumber = dto.PhoneNumber;
            await _userManager.UpdateAsync(resident.ApplicationUser);
        }

        _residentRepository.Update(resident);
        await _residentRepository.SaveChangesAsync();

        return _mapper.Map<ResidentDto>(resident);
    }

    public async Task DeleteAsync(int id)
    {
        var resident = await _residentRepository.GetByIdWithUserAsync(id);
        if (resident is null)
        {
            throw new NotFoundException("Resident", id);
        }

        // Deleting the Identity user cascades to the Resident row (and its requests/complaints)
        // via the FK relationship configured in ApplicationDbContext.
        if (resident.ApplicationUser is not null)
        {
            await _userManager.DeleteAsync(resident.ApplicationUser);
        }
        else
        {
            _residentRepository.Delete(resident);
            await _residentRepository.SaveChangesAsync();
        }
    }
}
