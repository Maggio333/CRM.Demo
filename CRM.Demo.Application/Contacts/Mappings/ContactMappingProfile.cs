using AutoMapper;
using CRM.Demo.Domain.Contacts.Entities;
using CRM.Demo.Application.Contacts.DTOs;

namespace CRM.Demo.Application.Contacts.Mappings;

/// <summary>
/// AutoMapper profile dla Contact.
/// Definiuje mapowania między Entity a DTOs.
/// </summary>
public class ContactMappingProfile : Profile
{
    public ContactMappingProfile()
    {
        // Contact Entity → ContactDto
        CreateMap<Contact, ContactDto>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email.Value))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber != null ? src.PhoneNumber.FullNumber : null))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.Value))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.Value))
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role != null ? src.Role.Value : null));
    }
}
