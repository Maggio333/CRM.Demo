using MediatR;
using CRM.Demo.Application.Tasks.DTOs;
using CRM.Demo.Domain.Common;
using Unit = MediatR.Unit;

namespace CRM.Demo.Application.Tasks.Commands.UpdateTask;

/// <summary>
/// Command dla aktualizacji Task.
/// </summary>
public class UpdateTaskCommand : IRequest<Result<Unit, string>>
{
    public Guid Id { get; set; }
    public UpdateTaskDto Dto { get; set; } = null!;
}
