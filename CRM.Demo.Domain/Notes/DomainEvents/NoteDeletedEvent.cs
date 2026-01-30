using CRM.Demo.Domain.Common;

namespace CRM.Demo.Domain.Notes.DomainEvents;

public class NoteDeletedEvent : IDomainEvent
{
    public Guid NoteId { get; }
    public Guid DeletedByUserId { get; }
    public DateTime DeletedAt { get; }
    public DateTime OccurredOn { get; }

    public NoteDeletedEvent(
        Guid noteId,
        Guid deletedByUserId,
        DateTime deletedAt)
    {
        NoteId = noteId;
        DeletedByUserId = deletedByUserId;
        DeletedAt = deletedAt;
        OccurredOn = DateTime.UtcNow;
    }
}
