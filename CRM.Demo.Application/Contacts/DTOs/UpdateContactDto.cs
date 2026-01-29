namespace CRM.Demo.Application.Contacts.DTOs;

/// <summary>
/// DTO dla aktualizacji Contact.
/// </summary>
public class UpdateContactDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? JobTitle { get; set; }
    public string? Department { get; set; }
    public Guid? CustomerId { get; set; }
}
