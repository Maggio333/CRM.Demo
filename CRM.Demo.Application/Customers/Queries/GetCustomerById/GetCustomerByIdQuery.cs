using MediatR;
using CRM.Demo.Application.Customers.DTOs;
using CRM.Demo.Domain.Common;

namespace CRM.Demo.Application.Customers.Queries.GetCustomerById;

/// <summary>
/// Query dla pobrania Customer po ID.
/// </summary>
public class GetCustomerByIdQuery : IRequest<Result<CustomerDto, string>>
{
    public Guid CustomerId { get; set; }
    
    public GetCustomerByIdQuery(Guid customerId)
    {
        CustomerId = customerId;
    }
}
