namespace CRM.Demo.Application.Customers.DTOs;

/// <summary>
/// DTO dla aktualizacji Customer - używany w requestach API.
/// </summary>
public class UpdateCustomerDto
{
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? Street { get; set; }
    public string? City { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
}
