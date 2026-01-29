using MediatR;
using CRM.Demo.Application.Tasks.DTOs;

namespace CRM.Demo.Application.Tasks.Queries.GetTasksList;

/// <summary>
/// Query dla pobrania listy Task z paginacją.
/// </summary>
public class GetTasksListQuery : IRequest<List<TaskDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public Guid? CustomerId { get; set; }
    public Guid? ContactId { get; set; }
    public Guid? AssignedToUserId { get; set; }
    public string? Status { get; set; }
}
