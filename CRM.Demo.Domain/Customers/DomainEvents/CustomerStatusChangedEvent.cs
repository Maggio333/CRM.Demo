using CRM.Demo.Domain.Common;
using CRM.Demo.Domain.Customers.ValueObjects;

namespace CRM.Demo.Domain.Customers.DomainEvents;

public class CustomerStatusChangedEvent : IDomainEvent
{
    public Guid CustomerId { get; }
    public string OldStatus { get; }
    public string NewStatus { get; }
    public DateTime ChangedAt { get; }
    public DateTime OccurredOn { get; }

    public CustomerStatusChangedEvent(
        Guid customerId,
        CustomerStatus oldStatus,
        CustomerStatus newStatus,
        DateTime changedAt)
    {
        CustomerId = customerId;
        OldStatus = oldStatus.Value;
        NewStatus = newStatus.Value;
        ChangedAt = changedAt;
        OccurredOn = DateTime.UtcNow;
    }
}
