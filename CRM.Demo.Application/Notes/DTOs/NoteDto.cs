namespace CRM.Demo.Application.Notes.DTOs;

/// <summary>
/// DTO dla Note - używany w odpowiedziach API.
/// </summary>
public class NoteDto
{
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? Title { get; set; }
    public string Type { get; set; } = string.Empty;
    public string? Category { get; set; }
    public Guid? CustomerId { get; set; }
    public Guid? ContactId { get; set; }
    public Guid? TaskId { get; set; }
    public Guid CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedByUserId { get; set; }
    public int AttachmentsCount { get; set; }
}
