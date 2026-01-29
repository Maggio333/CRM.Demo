using MediatR;
using AutoMapper;
using CRM.Demo.Domain.Common;
using CRM.Demo.Application.Contacts.DTOs;
using CRM.Demo.Application.Contacts.Interfaces;

namespace CRM.Demo.Application.Contacts.Queries.GetContactById;

/// <summary>
/// Handler dla GetContactByIdQuery.
/// Pobiera Contact po ID i mapuje na DTO.
/// </summary>
public class GetContactByIdQueryHandler : IRequestHandler<GetContactByIdQuery, Result<ContactDto, string>>
{
    private readonly IContactRepository _repository;
    private readonly IMapper _mapper;
    
    public GetContactByIdQueryHandler(
        IContactRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }
    
    public async Task<Result<ContactDto, string>> Handle(
        GetContactByIdQuery request,
        CancellationToken cancellationToken)
    {
        var contact = await _repository.GetByIdAsync(request.ContactId, cancellationToken);
        
        if (contact == null)
        {
            return Result<ContactDto, string>.Failure(
                $"Contact with ID {request.ContactId} not found"
            );
        }
        
        var contactDto = _mapper.Map<ContactDto>(contact);
        return Result<ContactDto, string>.Success(contactDto);
    }
}
