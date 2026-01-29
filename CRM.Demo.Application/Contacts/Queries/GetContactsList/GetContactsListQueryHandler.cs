using MediatR;
using AutoMapper;
using CRM.Demo.Application.Contacts.DTOs;
using CRM.Demo.Application.Contacts.Interfaces;

namespace CRM.Demo.Application.Contacts.Queries.GetContactsList;

/// <summary>
/// Handler dla GetContactsListQuery.
/// Pobiera listę Contact z paginacją i mapuje na DTOs.
/// </summary>
public class GetContactsListQueryHandler : IRequestHandler<GetContactsListQuery, List<ContactDto>>
{
    private readonly IContactRepository _repository;
    private readonly IMapper _mapper;
    
    public GetContactsListQueryHandler(
        IContactRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }
    
    public async Task<List<ContactDto>> Handle(
        GetContactsListQuery request,
        CancellationToken cancellationToken)
    {
        List<Domain.Contacts.Entities.Contact> contacts;
        
        if (request.CustomerId.HasValue)
        {
            contacts = await _repository.GetByCustomerIdAsync(request.CustomerId.Value, cancellationToken);
        }
        else if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            contacts = await _repository.SearchByNameAsync(request.SearchTerm, cancellationToken);
        }
        else
        {
            contacts = await _repository.GetAllAsync(cancellationToken);
        }
        
        // Paginacja
        var paginatedContacts = contacts
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();
        
        return _mapper.Map<List<ContactDto>>(paginatedContacts);
    }
}
