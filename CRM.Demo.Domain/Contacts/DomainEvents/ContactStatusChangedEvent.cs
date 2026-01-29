using CRM.Demo.Domain.Common;
using CRM.Demo.Domain.Contacts.ValueObjects;

namespace CRM.Demo.Domain.Contacts.DomainEvents;

public class ContactStatusChangedEvent : IDomainEvent
{
    public Guid ContactId { get; }
    public string OldStatus { get; }
    public string NewStatus { get; }
    public DateTime ChangedAt { get; }
    public DateTime OccurredOn { get; }
    
    public ContactStatusChangedEvent(
        Guid contactId,
        ContactStatus oldStatus,
        ContactStatus newStatus,
        DateTime changedAt)
    {
        ContactId = contactId;
        OldStatus = oldStatus.Value;
        NewStatus = newStatus.Value;
        ChangedAt = changedAt;
        OccurredOn = DateTime.UtcNow;
    }
}
