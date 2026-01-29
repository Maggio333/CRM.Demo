using CRM.Demo.Domain.Common;

namespace CRM.Demo.Domain.Notes.DomainEvents;

public class NoteCreatedEvent : IDomainEvent
{
    public Guid NoteId { get; }
    public string NoteType { get; }
    public Guid? CustomerId { get; }
    public Guid? ContactId { get; }
    public Guid? TaskId { get; }
    public DateTime CreatedAt { get; }
    public DateTime OccurredOn { get; }
    
    public NoteCreatedEvent(
        Guid noteId,
        string noteType,
        DateTime createdAt,
        Guid? customerId = null,
        Guid? contactId = null,
        Guid? taskId = null)
    {
        NoteId = noteId;
        NoteType = noteType;
        CustomerId = customerId;
        ContactId = contactId;
        TaskId = taskId;
        CreatedAt = createdAt;
        OccurredOn = DateTime.UtcNow;
    }
}
