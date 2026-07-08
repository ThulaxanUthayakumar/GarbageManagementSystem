using AutoMapper;
using GarbageManagementSystem.API.Common;
using GarbageManagementSystem.API.DTOs.Common;
using GarbageManagementSystem.API.DTOs.Schedules;
using GarbageManagementSystem.API.Models.Entities;
using GarbageManagementSystem.API.Repositories;

namespace GarbageManagementSystem.API.Services;

public interface IScheduleService
{
    Task<PagedResultDto<ScheduleDto>> GetPagedAsync(PaginationParams pagination, string? zone);
    Task<List<ScheduleDto>> GetByZoneAsync(string zone);
    Task<ScheduleDto> GetByIdAsync(int id);
    Task<ScheduleDto> CreateAsync(CreateScheduleDto dto);
    Task<ScheduleDto> UpdateAsync(int id, UpdateScheduleDto dto);
    Task DeleteAsync(int id);
}

public class ScheduleService : IScheduleService
{
    private readonly IScheduleRepository _scheduleRepository;
    private readonly IWasteCategoryRepository _wasteCategoryRepository;
    private readonly IMapper _mapper;

    public ScheduleService(IScheduleRepository scheduleRepository, IWasteCategoryRepository wasteCategoryRepository, IMapper mapper)
    {
        _scheduleRepository = scheduleRepository;
        _wasteCategoryRepository = wasteCategoryRepository;
        _mapper = mapper;
    }

    public async Task<PagedResultDto<ScheduleDto>> GetPagedAsync(PaginationParams pagination, string? zone)
    {
        var (items, totalCount) = await _scheduleRepository.GetPagedAsync(pagination.PageNumber, pagination.PageSize, zone);

        return new PagedResultDto<ScheduleDto>
        {
            Items = _mapper.Map<List<ScheduleDto>>(items),
            TotalCount = totalCount,
            PageNumber = pagination.PageNumber,
            PageSize = pagination.PageSize
        };
    }

    public async Task<List<ScheduleDto>> GetByZoneAsync(string zone)
    {
        var schedules = await _scheduleRepository.GetByZoneAsync(zone);
        return _mapper.Map<List<ScheduleDto>>(schedules);
    }

    public async Task<ScheduleDto> GetByIdAsync(int id)
    {
        var schedule = await _scheduleRepository.GetByIdWithDetailsAsync(id);
        if (schedule is null)
        {
            throw new NotFoundException("Collection schedule", id);
        }

        return _mapper.Map<ScheduleDto>(schedule);
    }

    public async Task<ScheduleDto> CreateAsync(CreateScheduleDto dto)
    {
        if (dto.WasteCategoryId.HasValue)
        {
            var exists = await _wasteCategoryRepository.ExistsAsync(wc => wc.Id == dto.WasteCategoryId.Value);
            if (!exists)
            {
                throw new BadRequestException("The selected waste category does not exist.");
            }
        }

        var schedule = new CollectionSchedule
        {
            Zone = dto.Zone,
            WasteCategoryId = dto.WasteCategoryId,
            ScheduledDate = dto.ScheduledDate,
            ScheduledTime = dto.ScheduledTime,
            CollectorName = dto.CollectorName,
            Notes = dto.Notes,
            CreatedAt = DateTime.UtcNow
        };

        await _scheduleRepository.AddAsync(schedule);
        await _scheduleRepository.SaveChangesAsync();

        var created = await _scheduleRepository.GetByIdWithDetailsAsync(schedule.Id);
        return _mapper.Map<ScheduleDto>(created);
    }

    public async Task<ScheduleDto> UpdateAsync(int id, UpdateScheduleDto dto)
    {
        var schedule = await _scheduleRepository.GetByIdAsync(id);
        if (schedule is null)
        {
            throw new NotFoundException("Collection schedule", id);
        }

        schedule.Zone = dto.Zone;
        schedule.WasteCategoryId = dto.WasteCategoryId;
        schedule.ScheduledDate = dto.ScheduledDate;
        schedule.ScheduledTime = dto.ScheduledTime;
        schedule.CollectorName = dto.CollectorName;
        schedule.Status = dto.Status;
        schedule.Notes = dto.Notes;
        schedule.UpdatedAt = DateTime.UtcNow;

        _scheduleRepository.Update(schedule);
        await _scheduleRepository.SaveChangesAsync();

        var updated = await _scheduleRepository.GetByIdWithDetailsAsync(id);
        return _mapper.Map<ScheduleDto>(updated);
    }

    public async Task DeleteAsync(int id)
    {
        var schedule = await _scheduleRepository.GetByIdAsync(id);
        if (schedule is null)
        {
            throw new NotFoundException("Collection schedule", id);
        }

        _scheduleRepository.Delete(schedule);
        await _scheduleRepository.SaveChangesAsync();
    }
}
