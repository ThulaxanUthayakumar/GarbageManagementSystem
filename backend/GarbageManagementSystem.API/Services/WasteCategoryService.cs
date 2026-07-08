using AutoMapper;
using GarbageManagementSystem.API.Common;
using GarbageManagementSystem.API.DTOs.WasteCategories;
using GarbageManagementSystem.API.Models.Entities;
using GarbageManagementSystem.API.Repositories;

namespace GarbageManagementSystem.API.Services;

public interface IWasteCategoryService
{
    Task<List<WasteCategoryDto>> GetAllAsync();
    Task<WasteCategoryDto> GetByIdAsync(int id);
    Task<WasteCategoryDto> CreateAsync(CreateWasteCategoryDto dto);
    Task<WasteCategoryDto> UpdateAsync(int id, UpdateWasteCategoryDto dto);
    Task DeleteAsync(int id);
}

public class WasteCategoryService : IWasteCategoryService
{
    private readonly IWasteCategoryRepository _repository;
    private readonly IMapper _mapper;

    public WasteCategoryService(IWasteCategoryRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<List<WasteCategoryDto>> GetAllAsync()
    {
        var categories = await _repository.GetAllAsync();
        return _mapper.Map<List<WasteCategoryDto>>(categories.OrderBy(c => c.Name));
    }

    public async Task<WasteCategoryDto> GetByIdAsync(int id)
    {
        var category = await _repository.GetByIdAsync(id);
        if (category is null)
        {
            throw new NotFoundException("Waste category", id);
        }

        return _mapper.Map<WasteCategoryDto>(category);
    }

    public async Task<WasteCategoryDto> CreateAsync(CreateWasteCategoryDto dto)
    {
        if (await _repository.NameExistsAsync(dto.Name))
        {
            throw new BadRequestException($"A waste category named '{dto.Name}' already exists.");
        }

        var category = new WasteCategory
        {
            Name = dto.Name,
            Description = dto.Description,
            IconName = dto.IconName
        };

        await _repository.AddAsync(category);
        await _repository.SaveChangesAsync();

        return _mapper.Map<WasteCategoryDto>(category);
    }

    public async Task<WasteCategoryDto> UpdateAsync(int id, UpdateWasteCategoryDto dto)
    {
        var category = await _repository.GetByIdAsync(id);
        if (category is null)
        {
            throw new NotFoundException("Waste category", id);
        }

        if (await _repository.NameExistsAsync(dto.Name, id))
        {
            throw new BadRequestException($"A waste category named '{dto.Name}' already exists.");
        }

        category.Name = dto.Name;
        category.Description = dto.Description;
        category.IconName = dto.IconName;

        _repository.Update(category);
        await _repository.SaveChangesAsync();

        return _mapper.Map<WasteCategoryDto>(category);
    }

    public async Task DeleteAsync(int id)
    {
        var category = await _repository.GetByIdAsync(id);
        if (category is null)
        {
            throw new NotFoundException("Waste category", id);
        }

        _repository.Delete(category);
        await _repository.SaveChangesAsync();
    }
}
