namespace CRM.Demo.Application.Customers.DTOs;

/// <summary>
/// DTO dla Customer - używany w odpowiedziach API.
/// </summary>
public class CustomerDto
{
    public Guid Id { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string TaxId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string Status { get; set; } = string.Empty;
    public Guid? AssignedSalesRepId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
