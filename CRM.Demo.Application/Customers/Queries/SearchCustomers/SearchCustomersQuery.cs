using MediatR;
using CRM.Demo.Application.Customers.DTOs;

namespace CRM.Demo.Application.Customers.Queries.SearchCustomers;

/// <summary>
/// Query dla wyszukiwania Customer z użyciem Expression Trees.
/// </summary>
public class SearchCustomersQuery : IRequest<List<CustomerDto>>
{
    public string? CompanyName { get; set; }
    public string? Email { get; set; }
    public string? TaxId { get; set; }
    public string? Status { get; set; }
}
