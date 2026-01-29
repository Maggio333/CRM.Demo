using MediatR;
using CRM.Demo.Domain.Common;
using CRM.Demo.Domain.Tasks.ValueObjects;
using CRM.Demo.Application.Tasks.Interfaces;
using CRM.Demo.Application.Common.Interfaces;
using Unit = MediatR.Unit;

namespace CRM.Demo.Application.Tasks.Commands.UpdateTask;

/// <summary>
/// Handler dla UpdateTaskCommand.
/// Aktualizuje Task.
/// </summary>
public class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand, Result<Unit, string>>
{
    private readonly ITaskRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    
    public UpdateTaskCommandHandler(
        ITaskRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<Result<Unit, string>> Handle(
        UpdateTaskCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var task = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (task == null)
            {
                return Result<Unit, string>.Failure(
                    $"Task with ID {request.Id} not found"
                );
            }
            
            // Utwórz Value Objects
            var taskType = TaskType.FromString(request.Dto.Type);
            if (taskType == null)
            {
                return Result<Unit, string>.Failure("Invalid task type");
            }
            
            var priority = TaskPriority.FromString(request.Dto.Priority);
            if (priority == null)
            {
                return Result<Unit, string>.Failure("Invalid task priority");
            }
            
            // Aktualizuj Task - użyj nowej metody Update
            task.Update(
                request.Dto.Title,
                taskType,
                priority,
                request.Dto.Description,
                request.Dto.StartDate,
                request.Dto.DueDate,
                request.Dto.CustomerId,
                request.Dto.ContactId,
                request.Dto.AssignedToUserId
            );
            
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            return Result<Unit, string>.Success(Unit.Value);
        }
        catch (DomainException ex)
        {
            return Result<Unit, string>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            return Result<Unit, string>.Failure($"An error occurred: {ex.Message}");
        }
    }
}
