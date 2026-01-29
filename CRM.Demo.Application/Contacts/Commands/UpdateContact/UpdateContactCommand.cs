using MediatR;
using CRM.Demo.Application.Contacts.DTOs;
using CRM.Demo.Domain.Common;
using Unit = MediatR.Unit;

namespace CRM.Demo.Application.Contacts.Commands.UpdateContact;

/// <summary>
/// Command dla aktualizacji Contact.
/// </summary>
public class UpdateContactCommand : IRequest<Result<Unit, string>>
{
    public Guid Id { get; set; }
    public UpdateContactDto Dto { get; set; } = null!;
}
