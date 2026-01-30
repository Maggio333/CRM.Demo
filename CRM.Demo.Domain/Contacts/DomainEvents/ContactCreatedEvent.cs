using CRM.Demo.Domain.Common;

namespace CRM.Demo.Domain.Contacts.DomainEvents;

public class ContactCreatedEvent : IDomainEvent
{
    public Guid ContactId { get; }
    public string FullName { get; }
    public string Email { get; }
    public Guid? CustomerId { get; }
    public DateTime CreatedAt { get; }
    public DateTime OccurredOn { get; }

    public ContactCreatedEvent(
        Guid contactId,
        string fullName,
        string email,
        DateTime createdAt,
        Guid? customerId = null)
    {
        ContactId = contactId;
        FullName = fullName;
        Email = email;
        CustomerId = customerId;
        CreatedAt = createdAt;
        OccurredOn = DateTime.UtcNow;
    }
}
