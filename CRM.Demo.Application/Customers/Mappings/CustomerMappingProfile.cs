using AutoMapper;
using CRM.Demo.Domain.Customers.Entities;
using CRM.Demo.Application.Customers.DTOs;

namespace CRM.Demo.Application.Customers.Mappings;

/// <summary>
/// AutoMapper profile dla Customer.
/// Definiuje mapowania między Entity a DTOs.
/// </summary>
public class CustomerMappingProfile : Profile
{
    public CustomerMappingProfile()
    {
        // Customer Entity → CustomerDto
        CreateMap<Customer, CustomerDto>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email.Value))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber != null ? src.PhoneNumber.FullNumber : null))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address != null ? src.Address.ToString() : null))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.Value));
    }
}
