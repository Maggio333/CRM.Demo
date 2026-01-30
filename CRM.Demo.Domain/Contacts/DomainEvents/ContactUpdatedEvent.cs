using CRM.Demo.Domain.Common;

namespace CRM.Demo.Domain.Contacts.DomainEvents;

public class ContactUpdatedEvent : IDomainEvent
{
    public Guid ContactId { get; }
    public string ChangeDescription { get; }
    public DateTime UpdatedAt { get; }
    public DateTime OccurredOn { get; }

    public ContactUpdatedEvent(
        Guid contactId,
        string changeDescription,
        DateTime updatedAt)
    {
        ContactId = contactId;
        ChangeDescription = changeDescription;
        UpdatedAt = updatedAt;
        OccurredOn = DateTime.UtcNow;
    }
}
