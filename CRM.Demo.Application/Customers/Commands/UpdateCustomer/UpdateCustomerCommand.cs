using MediatR;
using CRM.Demo.Domain.Common;
using Unit = MediatR.Unit;

namespace CRM.Demo.Application.Customers.Commands.UpdateCustomer;

/// <summary>
/// Command dla aktualizacji Customer.
/// </summary>
public class UpdateCustomerCommand : IRequest<Result<Unit, string>>
{
    public Guid CustomerId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? Street { get; set; }
    public string? City { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
}
