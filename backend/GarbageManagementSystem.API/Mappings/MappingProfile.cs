using AutoMapper;
using GarbageManagementSystem.API.DTOs.Complaints;
using GarbageManagementSystem.API.DTOs.CollectionRequests;
using GarbageManagementSystem.API.DTOs.Residents;
using GarbageManagementSystem.API.DTOs.Schedules;
using GarbageManagementSystem.API.DTOs.WasteCategories;
using GarbageManagementSystem.API.Models.Entities;

namespace GarbageManagementSystem.API.Mappings;

/// <summary>
/// Central place for every Entity &lt;-&gt; DTO mapping in the application.
/// Keeping them all in one profile makes it easy to see the whole shape of the API at a glance.
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Resident, ResidentDto>()
            .ForMember(d => d.FullName, o => o.MapFrom(s => s.ApplicationUser != null ? s.ApplicationUser.FullName : string.Empty))
            .ForMember(d => d.Email, o => o.MapFrom(s => s.ApplicationUser != null ? s.ApplicationUser.Email : string.Empty))
            .ForMember(d => d.PhoneNumber, o => o.MapFrom(s => s.ApplicationUser != null ? s.ApplicationUser.PhoneNumber : string.Empty))
            .ForMember(d => d.TotalRequests, o => o.MapFrom(s => s.CollectionRequests.Count))
            .ForMember(d => d.TotalComplaints, o => o.MapFrom(s => s.Complaints.Count));

        CreateMap<WasteCategory, WasteCategoryDto>();

        CreateMap<CollectionRequest, CollectionRequestDto>()
            .ForMember(d => d.ResidentName, o => o.MapFrom(s => s.Resident != null && s.Resident.ApplicationUser != null ? s.Resident.ApplicationUser.FullName : string.Empty))
            .ForMember(d => d.Zone, o => o.MapFrom(s => s.Resident != null ? s.Resident.Zone : string.Empty))
            .ForMember(d => d.WasteCategoryName, o => o.MapFrom(s => s.WasteCategory != null ? s.WasteCategory.Name : string.Empty));

        CreateMap<CollectionSchedule, ScheduleDto>()
            .ForMember(d => d.WasteCategoryName, o => o.MapFrom(s => s.WasteCategory != null ? s.WasteCategory.Name : "All Categories"));

        CreateMap<Complaint, ComplaintDto>()
            .ForMember(d => d.ResidentName, o => o.MapFrom(s => s.Resident != null && s.Resident.ApplicationUser != null ? s.Resident.ApplicationUser.FullName : string.Empty))
            .ForMember(d => d.Zone, o => o.MapFrom(s => s.Resident != null ? s.Resident.Zone : string.Empty));
    }
}
