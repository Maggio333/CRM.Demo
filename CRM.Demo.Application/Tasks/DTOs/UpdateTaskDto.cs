namespace CRM.Demo.Application.Tasks.DTOs;

/// <summary>
/// DTO dla aktualizacji Task.
/// </summary>
public class UpdateTaskDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public DateTime? DueDate { get; set; }
    public DateTime? StartDate { get; set; }
    public Guid? CustomerId { get; set; }
    public Guid? ContactId { get; set; }
    public Guid? AssignedToUserId { get; set; }
}
