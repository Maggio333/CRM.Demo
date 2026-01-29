using MediatR;
using CRM.Demo.Application.Contacts.DTOs;

namespace CRM.Demo.Application.Contacts.Queries.GetContactsList;

/// <summary>
/// Query dla pobrania listy Contact z paginacją.
/// </summary>
public class GetContactsListQuery : IRequest<List<ContactDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public Guid? CustomerId { get; set; }
}
