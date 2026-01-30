using MediatR;
using CRM.Demo.Application.Tasks.DTOs;
using CRM.Demo.Domain.Common;

namespace CRM.Demo.Application.Tasks.Queries.GetTaskById;

/// <summary>
/// Query dla pobrania Task po ID.
/// </summary>
public class GetTaskByIdQuery : IRequest<Result<TaskDto, string>>
{
    public Guid TaskId { get; set; }

    public GetTaskByIdQuery(Guid taskId)
    {
        TaskId = taskId;
    }
}
