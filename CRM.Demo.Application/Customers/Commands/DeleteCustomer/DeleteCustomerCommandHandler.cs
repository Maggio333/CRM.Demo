using MediatR;
using CRM.Demo.Domain.Common;
using CRM.Demo.Application.Customers.Interfaces;
using CRM.Demo.Application.Common.Interfaces;
using Unit = MediatR.Unit;

namespace CRM.Demo.Application.Customers.Commands.DeleteCustomer;

/// <summary>
/// Handler dla DeleteCustomerCommand.
/// Usuwa Customer z bazy danych.
/// </summary>
public class DeleteCustomerCommandHandler : IRequestHandler<DeleteCustomerCommand, Result<Unit, string>>
{
    private readonly ICustomerRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCustomerCommandHandler(
        ICustomerRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Unit, string>> Handle(
        DeleteCustomerCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var customer = await _repository.GetByIdAsync(request.CustomerId, cancellationToken);

            if (customer == null)
            {
                return Result<Unit, string>.Failure(
                    $"Customer with ID {request.CustomerId} not found"
                );
            }

            // Usuń Customer
            await _repository.DeleteAsync(customer, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Unit, string>.Success(Unit.Value);
        }
        catch (Exception ex)
        {
            return Result<Unit, string>.Failure($"An error occurred: {ex.Message}");
        }
    }
}
