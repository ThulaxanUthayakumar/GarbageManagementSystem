using AutoMapper;
using GarbageManagementSystem.API.Common;
using GarbageManagementSystem.API.DTOs.CollectionRequests;
using GarbageManagementSystem.API.DTOs.Common;
using GarbageManagementSystem.API.Models.Entities;
using GarbageManagementSystem.API.Models.Enums;
using GarbageManagementSystem.API.Repositories;

namespace GarbageManagementSystem.API.Services;

public interface ICollectionRequestService
{
    Task<PagedResultDto<CollectionRequestDto>> GetPagedAsync(PaginationParams pagination, RequestStatus? status);
    Task<List<CollectionRequestDto>> GetMyRequestsAsync(int residentId);
    Task<CollectionRequestDto> GetByIdAsync(int id);
    Task<CollectionRequestDto> CreateAsync(int residentId, CreateCollectionRequestDto dto);
    Task<CollectionRequestDto> UpdateStatusAsync(int id, UpdateCollectionRequestStatusDto dto);
    Task DeleteAsync(int id);
}

public class CollectionRequestService : ICollectionRequestService
{
    private readonly ICollectionRequestRepository _requestRepository;
    private readonly IWasteCategoryRepository _wasteCategoryRepository;
    private readonly IFileUploadService _fileUploadService;
    private readonly IMapper _mapper;

    public CollectionRequestService(
        ICollectionRequestRepository requestRepository,
        IWasteCategoryRepository wasteCategoryRepository,
        IFileUploadService fileUploadService,
        IMapper mapper)
    {
        _requestRepository = requestRepository;
        _wasteCategoryRepository = wasteCategoryRepository;
        _fileUploadService = fileUploadService;
        _mapper = mapper;
    }

    public async Task<PagedResultDto<CollectionRequestDto>> GetPagedAsync(PaginationParams pagination, RequestStatus? status)
    {
        var (items, totalCount) = await _requestRepository.GetPagedAsync(pagination.PageNumber, pagination.PageSize, pagination.SearchTerm, status);

        return new PagedResultDto<CollectionRequestDto>
        {
            Items = _mapper.Map<List<CollectionRequestDto>>(items),
            TotalCount = totalCount,
            PageNumber = pagination.PageNumber,
            PageSize = pagination.PageSize
        };
    }

    public async Task<List<CollectionRequestDto>> GetMyRequestsAsync(int residentId)
    {
        var requests = await _requestRepository.GetByResidentIdAsync(residentId);
        return _mapper.Map<List<CollectionRequestDto>>(requests);
    }

    public async Task<CollectionRequestDto> GetByIdAsync(int id)
    {
        var request = await _requestRepository.GetByIdWithDetailsAsync(id);
        if (request is null)
        {
            throw new NotFoundException("Collection request", id);
        }

        return _mapper.Map<CollectionRequestDto>(request);
    }

    public async Task<CollectionRequestDto> CreateAsync(int residentId, CreateCollectionRequestDto dto)
    {
        var categoryExists = await _wasteCategoryRepository.ExistsAsync(wc => wc.Id == dto.WasteCategoryId);
        if (!categoryExists)
        {
            throw new BadRequestException("The selected waste category does not exist.");
        }

        var imageUrl = await _fileUploadService.SaveImageAsync(dto.Image, "requests");

        var request = new CollectionRequest
        {
            ResidentId = residentId,
            WasteCategoryId = dto.WasteCategoryId,
            Description = dto.Description,
            ImageUrl = imageUrl,
            PickupDate = dto.PickupDate,
            Status = RequestStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        await _requestRepository.AddAsync(request);
        await _requestRepository.SaveChangesAsync();

        var created = await _requestRepository.GetByIdWithDetailsAsync(request.Id);
        return _mapper.Map<CollectionRequestDto>(created);
    }

    public async Task<CollectionRequestDto> UpdateStatusAsync(int id, UpdateCollectionRequestStatusDto dto)
    {
        var request = await _requestRepository.GetByIdWithDetailsAsync(id);
        if (request is null)
        {
            throw new NotFoundException("Collection request", id);
        }

        request.Status = dto.Status;
        request.UpdatedAt = DateTime.UtcNow;
        request.CompletedDate = dto.Status == RequestStatus.Completed ? DateTime.UtcNow : request.CompletedDate;

        _requestRepository.Update(request);
        await _requestRepository.SaveChangesAsync();

        return _mapper.Map<CollectionRequestDto>(request);
    }

    public async Task DeleteAsync(int id)
    {
        var request = await _requestRepository.GetByIdAsync(id);
        if (request is null)
        {
            throw new NotFoundException("Collection request", id);
        }

        _fileUploadService.DeleteImage(request.ImageUrl);
        _requestRepository.Delete(request);
        await _requestRepository.SaveChangesAsync();
    }
}
