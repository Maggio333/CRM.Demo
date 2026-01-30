using CRM.Demo.Domain.Common;

namespace CRM.Demo.Domain.Tasks.DomainEvents;

public class TaskAssignedEvent : IDomainEvent
{
    public Guid TaskId { get; }
    public Guid? OldUserId { get; }
    public Guid NewUserId { get; }
    public DateTime AssignedAt { get; }
    public DateTime OccurredOn { get; }

    public TaskAssignedEvent(
        Guid taskId,
        Guid? oldUserId,
        Guid newUserId,
        DateTime assignedAt)
    {
        TaskId = taskId;
        OldUserId = oldUserId;
        NewUserId = newUserId;
        AssignedAt = assignedAt;
        OccurredOn = DateTime.UtcNow;
    }
}
