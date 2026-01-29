using MediatR;
using CRM.Demo.Domain.Common;
using Unit = MediatR.Unit;

namespace CRM.Demo.Application.Customers.Commands.DeleteCustomer;

/// <summary>
/// Command dla usunięcia Customer.
/// </summary>
public class DeleteCustomerCommand : IRequest<Result<Unit, string>>
{
    public Guid CustomerId { get; set; }
    
    public DeleteCustomerCommand(Guid customerId)
    {
        CustomerId = customerId;
    }
}
