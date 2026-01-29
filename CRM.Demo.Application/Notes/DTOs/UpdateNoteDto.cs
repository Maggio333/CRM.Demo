namespace CRM.Demo.Application.Notes.DTOs;

/// <summary>
/// DTO dla aktualizacji Note.
/// </summary>
public class UpdateNoteDto
{
    public string Content { get; set; } = string.Empty;
    public string? Title { get; set; }
    public string Type { get; set; } = string.Empty;
    public string? Category { get; set; }
    public Guid? CustomerId { get; set; }
    public Guid? ContactId { get; set; }
    public Guid? TaskId { get; set; }
}
