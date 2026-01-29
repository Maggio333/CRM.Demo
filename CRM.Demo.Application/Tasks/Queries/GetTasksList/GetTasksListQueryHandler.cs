using MediatR;
using AutoMapper;
using CRM.Demo.Application.Tasks.DTOs;
using CRM.Demo.Application.Tasks.Interfaces;
using DomainTask = CRM.Demo.Domain.Tasks.Entities.Task;

namespace CRM.Demo.Application.Tasks.Queries.GetTasksList;

/// <summary>
/// Handler dla GetTasksListQuery.
/// Pobiera listę Task z paginacją i mapuje na DTOs.
/// </summary>
public class GetTasksListQueryHandler : IRequestHandler<GetTasksListQuery, List<TaskDto>>
{
    private readonly ITaskRepository _repository;
    private readonly IMapper _mapper;
    
    public GetTasksListQueryHandler(
        ITaskRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }
    
    public async Task<List<TaskDto>> Handle(
        GetTasksListQuery request,
        CancellationToken cancellationToken)
    {
        List<Domain.Tasks.Entities.Task> tasks;
        
        if (request.CustomerId.HasValue)
        {
            tasks = await _repository.GetByCustomerIdAsync(request.CustomerId.Value, cancellationToken);
        }
        else if (request.ContactId.HasValue)
        {
            tasks = await _repository.GetByContactIdAsync(request.ContactId.Value, cancellationToken);
        }
        else if (request.AssignedToUserId.HasValue)
        {
            tasks = await _repository.GetByAssignedUserIdAsync(request.AssignedToUserId.Value, cancellationToken);
        }
        else
        {
            tasks = await _repository.GetAllAsync(cancellationToken);
        }
        
        // Filtrowanie po Status jeśli podano
        if (!string.IsNullOrEmpty(request.Status))
        {
            tasks = tasks.Where(t => t.Status.Value == request.Status).ToList();
        }
        
        // Paginacja
        var paginatedTasks = tasks
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();
        
        return _mapper.Map<List<TaskDto>>(paginatedTasks);
    }
}
