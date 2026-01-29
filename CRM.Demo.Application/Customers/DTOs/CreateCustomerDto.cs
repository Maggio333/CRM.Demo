namespace CRM.Demo.Application.Customers.DTOs;

/// <summary>
/// DTO dla tworzenia Customer - używany w requestach API.
/// </summary>
public class CreateCustomerDto
{
    public string CompanyName { get; set; } = string.Empty;
    public string TaxId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? Street { get; set; }
    public string? City { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
}
