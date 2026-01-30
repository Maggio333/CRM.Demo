using MediatR;
using CRM.Demo.Domain.Common;
using CRM.Demo.Domain.Tasks.Entities;
using CRM.Demo.Domain.Tasks.ValueObjects;
using CRM.Demo.Application.Tasks.Interfaces;
using CRM.Demo.Application.Common.Interfaces;

namespace CRM.Demo.Application.Tasks.Commands.CreateTask;

/// <summary>
/// Handler dla CreateTaskCommand.
/// Tworzy nowy Task i zapisuje do bazy.
/// </summary>
public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, Result<Guid, string>>
{
    private readonly ITaskRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateTaskCommandHandler(
        ITaskRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid, string>> Handle(
        CreateTaskCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Utwórz Value Objects
            var taskType = TaskType.FromString(request.Dto.Type);
            var priority = TaskPriority.FromString(request.Dto.Priority);

            // Utwórz Task (Entity)
            var task = Domain.Tasks.Entities.Task.Create(
                request.Dto.Title,
                taskType,
                priority,
                request.Dto.CreatedByUserId,
                request.Dto.Description,
                request.Dto.DueDate,
                request.Dto.StartDate,
                request.Dto.CustomerId,
                request.Dto.ContactId,
                request.Dto.AssignedToUserId
            );

            // Zapisz przez Repository
            await _repository.AddAsync(task, cancellationToken);

            // UnitOfWork zapisuje do bazy i publikuje Domain Events
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid, string>.Success(task.Id);
        }
        catch (DomainException ex)
        {
            return Result<Guid, string>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            return Result<Guid, string>.Failure($"An error occurred: {ex.Message}");
        }
    }
}
