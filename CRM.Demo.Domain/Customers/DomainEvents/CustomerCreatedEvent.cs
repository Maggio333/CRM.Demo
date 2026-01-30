using CRM.Demo.Domain.Common;

namespace CRM.Demo.Domain.Customers.DomainEvents;

public class CustomerCreatedEvent : IDomainEvent
{
    public Guid CustomerId { get; }
    public string CompanyName { get; }
    public string Email { get; }
    public DateTime CreatedAt { get; }
    public DateTime OccurredOn { get; }

    public CustomerCreatedEvent(
        Guid customerId,
        string companyName,
        string email,
        DateTime createdAt)
    {
        CustomerId = customerId;
        CompanyName = companyName;
        Email = email;
        CreatedAt = createdAt;
        OccurredOn = DateTime.UtcNow;
    }
}
