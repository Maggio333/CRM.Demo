using CRM.Demo.Domain.Common;
using CRM.Demo.Domain.Tasks.ValueObjects;
using TaskStatusValueObject = CRM.Demo.Domain.Tasks.ValueObjects.TaskStatus;

namespace CRM.Demo.Domain.Tasks.DomainEvents;

public class TaskStatusChangedEvent : IDomainEvent
{
    public Guid TaskId { get; }
    public string OldStatus { get; }
    public string NewStatus { get; }
    public DateTime ChangedAt { get; }
    public DateTime OccurredOn { get; }
    
    public TaskStatusChangedEvent(
        Guid taskId,
        TaskStatusValueObject oldStatus,
        TaskStatusValueObject newStatus,
        DateTime changedAt)
    {
        TaskId = taskId;
        OldStatus = oldStatus.Value;
        NewStatus = newStatus.Value;
        ChangedAt = changedAt;
        OccurredOn = DateTime.UtcNow;
    }
}
