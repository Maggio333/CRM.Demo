using MediatR;
using CRM.Demo.Domain.Common;
using Unit = MediatR.Unit;

namespace CRM.Demo.Application.Tasks.Commands.DeleteTask;

/// <summary>
/// Command dla usunięcia Task.
/// </summary>
public class DeleteTaskCommand : IRequest<Result<Unit, string>>
{
    public Guid TaskId { get; set; }
    
    public DeleteTaskCommand(Guid taskId)
    {
        TaskId = taskId;
    }
}
