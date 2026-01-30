using CRM.Demo.Domain.Common;

namespace CRM.Demo.Domain.Notes.DomainEvents;

public class NoteAttachmentAddedEvent : IDomainEvent
{
    public Guid NoteId { get; }
    public string FileName { get; }
    public DateTime AddedAt { get; }
    public DateTime OccurredOn { get; }

    public NoteAttachmentAddedEvent(
        Guid noteId,
        string fileName,
        DateTime addedAt)
    {
        NoteId = noteId;
        FileName = fileName;
        AddedAt = addedAt;
        OccurredOn = DateTime.UtcNow;
    }
}
