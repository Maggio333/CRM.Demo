using MediatR;
using CRM.Demo.Domain.Common;
using Unit = MediatR.Unit;

namespace CRM.Demo.Application.Contacts.Commands.DeleteContact;

/// <summary>
/// Command dla usunięcia Contact.
/// </summary>
public class DeleteContactCommand : IRequest<Result<Unit, string>>
{
    public Guid ContactId { get; set; }

    public DeleteContactCommand(Guid contactId)
    {
        ContactId = contactId;
    }
}
