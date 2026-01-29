using CRM.Demo.Domain.Common;

namespace CRM.Demo.Domain.Tasks.DomainEvents;

public class TaskCompletedEvent : IDomainEvent
{
    public Guid TaskId { get; }
    public DateTime CompletedDate { get; }
    public DateTime OccurredOn { get; }
    
    public TaskCompletedEvent(
        Guid taskId,
        DateTime completedDate)
    {
        TaskId = taskId;
        CompletedDate = completedDate;
        OccurredOn = DateTime.UtcNow;
    }
}
