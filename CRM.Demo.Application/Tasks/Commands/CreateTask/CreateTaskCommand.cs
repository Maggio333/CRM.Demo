using MediatR;
using CRM.Demo.Application.Tasks.DTOs;
using CRM.Demo.Domain.Common;

namespace CRM.Demo.Application.Tasks.Commands.CreateTask;

/// <summary>
/// Command dla tworzenia nowego Task.
/// </summary>
public class CreateTaskCommand : IRequest<Result<Guid, string>>
{
    public CreateTaskDto Dto { get; set; } = null!;
}
