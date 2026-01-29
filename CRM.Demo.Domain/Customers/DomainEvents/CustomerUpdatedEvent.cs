using CRM.Demo.Domain.Common;

namespace CRM.Demo.Domain.Customers.DomainEvents;

public class CustomerUpdatedEvent : IDomainEvent
{
    public Guid CustomerId { get; }
    public string ChangeDescription { get; }
    public DateTime UpdatedAt { get; }
    public DateTime OccurredOn { get; }
    
    public CustomerUpdatedEvent(
        Guid customerId,
        string changeDescription,
        DateTime updatedAt)
    {
        CustomerId = customerId;
        ChangeDescription = changeDescription;
        UpdatedAt = updatedAt;
        OccurredOn = DateTime.UtcNow;
    }
}
