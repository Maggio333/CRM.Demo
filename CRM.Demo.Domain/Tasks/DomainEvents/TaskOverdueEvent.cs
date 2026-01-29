using CRM.Demo.Domain.Common;

namespace CRM.Demo.Domain.Tasks.DomainEvents;

public class TaskOverdueEvent : IDomainEvent
{
    public Guid TaskId { get; }
    public DateTime DueDate { get; }
    public Guid? AssignedToUserId { get; }
    public DateTime OccurredOn { get; }
    
    public TaskOverdueEvent(
        Guid taskId,
        DateTime dueDate,
        Guid? assignedToUserId = null)
    {
        TaskId = taskId;
        DueDate = dueDate;
        AssignedToUserId = assignedToUserId;
        OccurredOn = DateTime.UtcNow;
    }
}
