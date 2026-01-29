using AutoMapper;
using CRM.Demo.Domain.Notes.Entities;
using CRM.Demo.Application.Notes.DTOs;

namespace CRM.Demo.Application.Notes.Mappings;

/// <summary>
/// AutoMapper profile dla Note.
/// Definiuje mapowania między Entity a DTOs.
/// </summary>
public class NoteMappingProfile : Profile
{
    public NoteMappingProfile()
    {
        // Note Entity → NoteDto
        CreateMap<Note, NoteDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.Value))
            .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category != null ? src.Category.Value : null))
            .ForMember(dest => dest.AttachmentsCount, opt => opt.MapFrom(src => src.Attachments.Count));
    }
}
