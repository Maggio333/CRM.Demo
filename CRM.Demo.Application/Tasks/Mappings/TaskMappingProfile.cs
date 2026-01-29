using AutoMapper;
using CRM.Demo.Domain.Tasks.Entities;
using CRM.Demo.Application.Tasks.DTOs;

namespace CRM.Demo.Application.Tasks.Mappings;

/// <summary>
/// AutoMapper profile dla Task.
/// Definiuje mapowania między Entity a DTOs.
/// </summary>
public class TaskMappingProfile : Profile
{
    public TaskMappingProfile()
    {
        // Task Entity → TaskDto
        CreateMap<Domain.Tasks.Entities.Task, TaskDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.Value))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.Value))
            .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.Priority.Value))
            .ForMember(dest => dest.IsOverdue, opt => opt.MapFrom(src => src.IsOverdue));
    }
}
