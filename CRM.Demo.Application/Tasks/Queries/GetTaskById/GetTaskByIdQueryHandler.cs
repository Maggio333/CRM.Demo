using MediatR;
using AutoMapper;
using CRM.Demo.Domain.Common;
using CRM.Demo.Application.Tasks.DTOs;
using CRM.Demo.Application.Tasks.Interfaces;

namespace CRM.Demo.Application.Tasks.Queries.GetTaskById;

/// <summary>
/// Handler dla GetTaskByIdQuery.
/// Pobiera Task po ID i mapuje na DTO.
/// </summary>
public class GetTaskByIdQueryHandler : IRequestHandler<GetTaskByIdQuery, Result<TaskDto, string>>
{
    private readonly ITaskRepository _repository;
    private readonly IMapper _mapper;
    
    public GetTaskByIdQueryHandler(
        ITaskRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }
    
    public async Task<Result<TaskDto, string>> Handle(
        GetTaskByIdQuery request,
        CancellationToken cancellationToken)
    {
        var task = await _repository.GetByIdAsync(request.TaskId, cancellationToken);
        
        if (task == null)
        {
            return Result<TaskDto, string>.Failure(
                $"Task with ID {request.TaskId} not found"
            );
        }
        
        var taskDto = _mapper.Map<TaskDto>(task);
        return Result<TaskDto, string>.Success(taskDto);
    }
}
