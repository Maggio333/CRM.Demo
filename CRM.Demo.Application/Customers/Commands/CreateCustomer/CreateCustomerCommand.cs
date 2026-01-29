using MediatR;
using CRM.Demo.Application.Customers.DTOs;
using CRM.Demo.Domain.Common;

namespace CRM.Demo.Application.Customers.Commands.CreateCustomer;

/// <summary>
/// Command dla tworzenia nowego Customer.
/// </summary>
public class CreateCustomerCommand : IRequest<Result<Guid, string>>
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
