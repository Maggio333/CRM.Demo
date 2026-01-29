using MediatR;
using CRM.Demo.Application.Customers.DTOs;

namespace CRM.Demo.Application.Customers.Queries.GetCustomersList;

/// <summary>
/// Query dla pobrania listy Customer z paginacją.
/// </summary>
public class GetCustomersListQuery : IRequest<List<CustomerDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
}
