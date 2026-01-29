using MediatR;
using CRM.Demo.Application.Contacts.DTOs;
using CRM.Demo.Domain.Common;

namespace CRM.Demo.Application.Contacts.Commands.CreateContact;

/// <summary>
/// Command dla tworzenia nowego Contact.
/// </summary>
public class CreateContactCommand : IRequest<Result<Guid, string>>
{
    public CreateContactDto Dto { get; set; } = null!;
}
