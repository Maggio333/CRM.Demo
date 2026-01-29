using CRM.Demo.Domain.Common;

namespace CRM.Demo.Domain.Tasks.DomainEvents;

public class TaskCreatedEvent : IDomainEvent
{
    public Guid TaskId { get; }
    public string Title { get; }
    public string TaskType { get; }
    public Guid? AssignedToUserId { get; }
    public Guid? CustomerId { get; }
    public Guid? ContactId { get; }
    public DateTime CreatedAt { get; }
    public DateTime OccurredOn { get; }
    
    public TaskCreatedEvent(
        Guid taskId,
        string title,
        string taskType,
        DateTime createdAt,
        Guid? assignedToUserId = null,
        Guid? customerId = null,
        Guid? contactId = null)
    {
        TaskId = taskId;
        Title = title;
        TaskType = taskType;
        AssignedToUserId = assignedToUserId;
        CustomerId = customerId;
        ContactId = contactId;
        CreatedAt = createdAt;
        OccurredOn = DateTime.UtcNow;
    }
}
