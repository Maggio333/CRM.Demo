namespace CRM.Demo.Application.Notes.DTOs;

/// <summary>
/// DTO dla tworzenia nowego Note.
/// </summary>
public class CreateNoteDto
{
    public string Content { get; set; } = string.Empty;
    public string? Title { get; set; }
    public string Type { get; set; } = string.Empty;
    public string? Category { get; set; }
    public Guid? CustomerId { get; set; }
    public Guid? ContactId { get; set; }
    public Guid? TaskId { get; set; }
    public Guid CreatedByUserId { get; set; }
}
