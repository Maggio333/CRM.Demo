using Microsoft.AspNetCore.Mvc;
using MediatR;
using CRM.Demo.Application.Tasks.Commands.CreateTask;
using CRM.Demo.Application.Tasks.Commands.UpdateTask;
using CRM.Demo.Application.Tasks.Commands.DeleteTask;
using CRM.Demo.Application.Tasks.Queries.GetTaskById;
using CRM.Demo.Application.Tasks.Queries.GetTasksList;
using CRM.Demo.Application.Tasks.DTOs;
using CRM.Demo.Domain.Common;

namespace CRM.Demo.Api.Controllers;

/// <summary>
/// Controller dla zarządzania Tasks.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly IMediator _mediator;

    public TasksController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Pobiera listę Tasks z filtrowaniem i paginacją.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<TaskDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetList(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] Guid? customerId = null,
        [FromQuery] Guid? contactId = null,
        [FromQuery] Guid? assignedToUserId = null,
        [FromQuery] string? status = null)
    {
        var query = new GetTasksListQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            CustomerId = customerId,
            ContactId = contactId,
            AssignedToUserId = assignedToUserId,
            Status = status
        };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Pobiera Task po ID.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TaskDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var query = new GetTaskByIdQuery(id);
        var result = await _mediator.Send(query);

        if (result.IsFailure)
        {
            return NotFound(result.Error);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Tworzy nowy Task.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateTaskDto dto)
    {
        var command = new CreateTaskCommand { Dto = dto };
        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value);
    }

    /// <summary>
    /// Aktualizuje Task.
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTaskDto dto)
    {
        if (dto == null)
        {
            return BadRequest("Request body is required");
        }

        var command = new UpdateTaskCommand { Id = id, Dto = dto };
        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            if (result.Error.Contains("not found"))
            {
                return NotFound(result.Error);
            }
            return BadRequest(new { message = result.Error });
        }

        return NoContent();
    }

    /// <summary>
    /// Usuwa Task.
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new DeleteTaskCommand(id);
        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            return NotFound(result.Error);
        }

        return NoContent();
    }
}
