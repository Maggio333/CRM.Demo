using CRM.Demo.Domain.Common;

namespace CRM.Demo.Domain.Notes.DomainEvents;

public class NoteUpdatedEvent : IDomainEvent
{
    public Guid NoteId { get; }
    public DateTime UpdatedAt { get; }
    public DateTime OccurredOn { get; }
    
    public NoteUpdatedEvent(
        Guid noteId,
        DateTime updatedAt)
    {
        NoteId = noteId;
        UpdatedAt = updatedAt;
        OccurredOn = DateTime.UtcNow;
    }
}
