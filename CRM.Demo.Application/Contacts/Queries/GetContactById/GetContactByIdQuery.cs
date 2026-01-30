using MediatR;
using CRM.Demo.Application.Contacts.DTOs;
using CRM.Demo.Domain.Common;

namespace CRM.Demo.Application.Contacts.Queries.GetContactById;

/// <summary>
/// Query dla pobrania Contact po ID.
/// </summary>
public class GetContactByIdQuery : IRequest<Result<ContactDto, string>>
{
    public Guid ContactId { get; set; }

    public GetContactByIdQuery(Guid contactId)
    {
        ContactId = contactId;
    }
}
