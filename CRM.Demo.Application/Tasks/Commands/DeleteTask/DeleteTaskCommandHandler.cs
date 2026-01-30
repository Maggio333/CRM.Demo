using MediatR;
using CRM.Demo.Domain.Common;
using CRM.Demo.Application.Tasks.Interfaces;
using CRM.Demo.Application.Common.Interfaces;
using Unit = MediatR.Unit;

namespace CRM.Demo.Application.Tasks.Commands.DeleteTask;

/// <summary>
/// Handler dla DeleteTaskCommand.
/// Usuwa Task z bazy danych.
/// </summary>
public class DeleteTaskCommandHandler : IRequestHandler<DeleteTaskCommand, Result<Unit, string>>
{
    private readonly ITaskRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteTaskCommandHandler(
        ITaskRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Unit, string>> Handle(
        DeleteTaskCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var task = await _repository.GetByIdAsync(request.TaskId, cancellationToken);

            if (task == null)
            {
                return Result<Unit, string>.Failure(
                    $"Task with ID {request.TaskId} not found"
                );
            }

            await _repository.DeleteAsync(task, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Unit, string>.Success(Unit.Value);
        }
        catch (Exception ex)
        {
            return Result<Unit, string>.Failure($"An error occurred: {ex.Message}");
        }
    }
}
