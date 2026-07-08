using AutoMapper;
using GarbageManagementSystem.API.Common;
using GarbageManagementSystem.API.DTOs.Common;
using GarbageManagementSystem.API.DTOs.Complaints;
using GarbageManagementSystem.API.Models.Entities;
using GarbageManagementSystem.API.Models.Enums;
using GarbageManagementSystem.API.Repositories;

namespace GarbageManagementSystem.API.Services;

public interface IComplaintService
{
    Task<PagedResultDto<ComplaintDto>> GetPagedAsync(PaginationParams pagination, ComplaintStatus? status);
    Task<List<ComplaintDto>> GetMyComplaintsAsync(int residentId);
    Task<ComplaintDto> GetByIdAsync(int id);
    Task<ComplaintDto> CreateAsync(int residentId, CreateComplaintDto dto);
    Task<ComplaintDto> UpdateStatusAsync(int id, UpdateComplaintStatusDto dto);
    Task DeleteAsync(int id);
}

public class ComplaintService : IComplaintService
{
    private readonly IComplaintRepository _complaintRepository;
    private readonly IFileUploadService _fileUploadService;
    private readonly IMapper _mapper;

    public ComplaintService(IComplaintRepository complaintRepository, IFileUploadService fileUploadService, IMapper mapper)
    {
        _complaintRepository = complaintRepository;
        _fileUploadService = fileUploadService;
        _mapper = mapper;
    }

    public async Task<PagedResultDto<ComplaintDto>> GetPagedAsync(PaginationParams pagination, ComplaintStatus? status)
    {
        var (items, totalCount) = await _complaintRepository.GetPagedAsync(pagination.PageNumber, pagination.PageSize, pagination.SearchTerm, status);

        return new PagedResultDto<ComplaintDto>
        {
            Items = _mapper.Map<List<ComplaintDto>>(items),
            TotalCount = totalCount,
            PageNumber = pagination.PageNumber,
            PageSize = pagination.PageSize
        };
    }

    public async Task<List<ComplaintDto>> GetMyComplaintsAsync(int residentId)
    {
        var complaints = await _complaintRepository.GetByResidentIdAsync(residentId);
        return _mapper.Map<List<ComplaintDto>>(complaints);
    }

    public async Task<ComplaintDto> GetByIdAsync(int id)
    {
        var complaint = await _complaintRepository.GetByIdWithDetailsAsync(id);
        if (complaint is null)
        {
            throw new NotFoundException("Complaint", id);
        }

        return _mapper.Map<ComplaintDto>(complaint);
    }

    public async Task<ComplaintDto> CreateAsync(int residentId, CreateComplaintDto dto)
    {
        var imageUrl = await _fileUploadService.SaveImageAsync(dto.Image, "complaints");

        var complaint = new Complaint
        {
            ResidentId = residentId,
            Subject = dto.Subject,
            Description = dto.Description,
            ImageUrl = imageUrl,
            Status = ComplaintStatus.Open,
            CreatedAt = DateTime.UtcNow
        };

        await _complaintRepository.AddAsync(complaint);
        await _complaintRepository.SaveChangesAsync();

        var created = await _complaintRepository.GetByIdWithDetailsAsync(complaint.Id);
        return _mapper.Map<ComplaintDto>(created);
    }

    public async Task<ComplaintDto> UpdateStatusAsync(int id, UpdateComplaintStatusDto dto)
    {
        var complaint = await _complaintRepository.GetByIdWithDetailsAsync(id);
        if (complaint is null)
        {
            throw new NotFoundException("Complaint", id);
        }

        complaint.Status = dto.Status;
        complaint.AdminRemarks = dto.AdminRemarks ?? complaint.AdminRemarks;
        complaint.UpdatedAt = DateTime.UtcNow;

        if (dto.Status is ComplaintStatus.Resolved or ComplaintStatus.Closed)
        {
            complaint.ResolvedDate ??= DateTime.UtcNow;
        }

        _complaintRepository.Update(complaint);
        await _complaintRepository.SaveChangesAsync();

        return _mapper.Map<ComplaintDto>(complaint);
    }

    public async Task DeleteAsync(int id)
    {
        var complaint = await _complaintRepository.GetByIdAsync(id);
        if (complaint is null)
        {
            throw new NotFoundException("Complaint", id);
        }

        _fileUploadService.DeleteImage(complaint.ImageUrl);
        _complaintRepository.Delete(complaint);
        await _complaintRepository.SaveChangesAsync();
    }
}
