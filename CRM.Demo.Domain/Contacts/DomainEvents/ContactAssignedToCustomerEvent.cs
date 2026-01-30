using CRM.Demo.Domain.Common;

namespace CRM.Demo.Domain.Contacts.DomainEvents;

public class ContactAssignedToCustomerEvent : IDomainEvent
{
    public Guid ContactId { get; }
    public Guid? OldCustomerId { get; }
    public Guid? NewCustomerId { get; }
    public DateTime AssignedAt { get; }
    public DateTime OccurredOn { get; }

    public ContactAssignedToCustomerEvent(
        Guid contactId,
        Guid? oldCustomerId,
        Guid? newCustomerId,
        DateTime assignedAt)
    {
        ContactId = contactId;
        OldCustomerId = oldCustomerId;
        NewCustomerId = newCustomerId;
        AssignedAt = assignedAt;
        OccurredOn = DateTime.UtcNow;
    }
}
