using MediatR;
using CRM.Demo.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace CRM.Demo.Application.Common.Behaviors;

/// <summary>
/// Pipeline Behavior dla zarządzania transakcjami.
/// Automatycznie rozpoczyna i zatwierdza transakcje dla Commands.
/// </summary>
public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<TransactionBehavior<TRequest, TResponse>> _logger;

    public TransactionBehavior(
        IUnitOfWork unitOfWork,
        ILogger<TransactionBehavior<TRequest, TResponse>> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // Tylko dla Commands (nie Queries)
        // Można sprawdzić czy request implementuje marker interface
        var requestName = typeof(TRequest).Name;

        try
        {
            _logger.LogInformation(
                "Beginning transaction for {RequestName}",
                requestName
            );

            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            var response = await next();

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            _logger.LogInformation(
                "Transaction committed for {RequestName}",
                requestName
            );

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error in transaction for {RequestName}, rolling back",
                requestName
            );

            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
