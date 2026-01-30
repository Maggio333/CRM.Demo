using CRM.Demo.Domain.Common;
using CRM.Demo.Domain.Notes.ValueObjects;
using CRM.Demo.Domain.Notes.DomainEvents;

namespace CRM.Demo.Domain.Notes.Entities;

public class Note : Entity<Guid>
{
    // Podstawowe informacje
    public string Content { get; private set; }
    public string? Title { get; private set; }

    // Value Objects
    public NoteType Type { get; private set; }
    public NoteCategory? Category { get; private set; }

    // Relacje (tylko ID - referencje do innych agregatów)
    public Guid? CustomerId { get; private set; }
    public Guid? ContactId { get; private set; }
    public Guid? TaskId { get; private set; }

    // Załączniki
    private readonly List<NoteAttachment> _attachments = new();
    public IReadOnlyCollection<NoteAttachment> Attachments => _attachments;

    // Metadata
    public DateTime CreatedAt { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public Guid? UpdatedByUserId { get; private set; }

    // Prywatny konstruktor
    private Note() { }

    // Factory method
    public static Note Create(
        string content,
        NoteType type,
        Guid createdByUserId,
        string? title = null,
        NoteCategory? category = null,
        Guid? customerId = null,
        Guid? contactId = null,
        Guid? taskId = null)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new DomainException("Note content cannot be empty");

        if (content.Length > 10000)
            throw new DomainException("Note content is too long (max 10000 characters)");

        // Walidacja - notatka musi być powiązana z przynajmniej jednym obiektem
        if (customerId == null && contactId == null && taskId == null)
            throw new DomainException("Note must be linked to Customer, Contact, or Task");

        var note = new Note
        {
            Id = Guid.NewGuid(),
            Content = content,
            Title = title,
            Type = type,
            Category = category,
            CustomerId = customerId,
            ContactId = contactId,
            TaskId = taskId,
            CreatedByUserId = createdByUserId,
            CreatedAt = DateTime.UtcNow
        };

        note.AddDomainEvent(new NoteCreatedEvent(
            note.Id,
            note.Type.Value,
            note.CreatedAt,
            note.CustomerId,
            note.ContactId,
            note.TaskId
        ));

        return note;
    }

    // Metody biznesowe
    public void UpdateContent(string newContent, Guid updatedByUserId)
    {
        if (string.IsNullOrWhiteSpace(newContent))
            throw new DomainException("Note content cannot be empty");

        if (newContent.Length > 10000)
            throw new DomainException("Note content is too long (max 10000 characters)");

        Content = newContent;
        UpdatedAt = DateTime.UtcNow;
        UpdatedByUserId = updatedByUserId;

        AddDomainEvent(new NoteUpdatedEvent(
            Id,
            DateTime.UtcNow
        ));
    }

    public void UpdateTitle(string? newTitle, Guid updatedByUserId)
    {
        Title = newTitle;
        UpdatedAt = DateTime.UtcNow;
        UpdatedByUserId = updatedByUserId;
    }

    public void ChangeCategory(NoteCategory? category, Guid updatedByUserId)
    {
        Category = category;
        UpdatedAt = DateTime.UtcNow;
        UpdatedByUserId = updatedByUserId;
    }

    public void LinkToCustomer(Guid customerId)
    {
        CustomerId = customerId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void LinkToContact(Guid contactId)
    {
        ContactId = contactId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void LinkToTask(Guid taskId)
    {
        TaskId = taskId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddAttachment(NoteAttachment attachment)
    {
        if (_attachments.Count >= 10)
            throw new DomainException("Maximum 10 attachments per note");

        _attachments.Add(attachment);

        AddDomainEvent(new NoteAttachmentAddedEvent(
            Id,
            attachment.FileName,
            DateTime.UtcNow
        ));
    }

    public void RemoveAttachment(Guid attachmentId)
    {
        var attachment = _attachments.FirstOrDefault(a => a.Id == attachmentId);
        if (attachment != null)
        {
            _attachments.Remove(attachment);
        }
    }

    // Soft delete - nie usuwamy fizycznie, tylko oznaczamy
    public void Delete(Guid deletedByUserId)
    {
        AddDomainEvent(new NoteDeletedEvent(
            Id,
            deletedByUserId,
            DateTime.UtcNow
        ));
    }

    // Metoda do pełnej aktualizacji
    public void Update(
        string content,
        NoteType type,
        string? title,
        NoteCategory? category,
        Guid? customerId,
        Guid? contactId,
        Guid? taskId,
        Guid updatedByUserId)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new DomainException("Note content cannot be empty");

        if (content.Length > 10000)
            throw new DomainException("Note content is too long (max 10000 characters)");

        // Walidacja - notatka musi być powiązana z przynajmniej jednym obiektem
        if (customerId == null && contactId == null && taskId == null)
            throw new DomainException("Note must be linked to Customer, Contact, or Task");

        Content = content;
        Type = type;
        Title = title;
        Category = category;

        if (CustomerId != customerId)
        {
            CustomerId = customerId;
        }

        if (ContactId != contactId)
        {
            ContactId = contactId;
        }

        if (TaskId != taskId)
        {
            TaskId = taskId;
        }

        UpdatedAt = DateTime.UtcNow;
        UpdatedByUserId = updatedByUserId;

        AddDomainEvent(new NoteUpdatedEvent(
            Id,
            DateTime.UtcNow
        ));
    }
}
