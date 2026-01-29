using Microsoft.AspNetCore.Mvc;
using MediatR;
using CRM.Demo.Application.Notes.Commands.CreateNote;
using CRM.Demo.Application.Notes.Commands.UpdateNote;
using CRM.Demo.Application.Notes.Commands.DeleteNote;
using CRM.Demo.Application.Notes.Queries.GetNoteById;
using CRM.Demo.Application.Notes.Queries.GetNotesList;
using CRM.Demo.Application.Notes.DTOs;
using CRM.Demo.Domain.Common;

namespace CRM.Demo.Api.Controllers;

/// <summary>
/// Controller dla zarządzania Notes.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class NotesController : ControllerBase
{
    private readonly IMediator _mediator;

    public NotesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Pobiera listę Notes z filtrowaniem i paginacją.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<NoteDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetList(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] Guid? customerId = null,
        [FromQuery] Guid? contactId = null,
        [FromQuery] Guid? taskId = null,
        [FromQuery] Guid? createdByUserId = null)
    {
        var query = new GetNotesListQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            CustomerId = customerId,
            ContactId = contactId,
            TaskId = taskId,
            CreatedByUserId = createdByUserId
        };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Pobiera Note po ID.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(NoteDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var query = new GetNoteByIdQuery(id);
        var result = await _mediator.Send(query);

        if (result.IsFailure)
        {
            return NotFound(result.Error);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Tworzy nowy Note.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateNoteDto dto)
    {
        var command = new CreateNoteCommand { Dto = dto };
        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value);
    }

    /// <summary>
    /// Aktualizuje Note.
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateNoteDto dto)
    {
        if (dto == null)
        {
            return BadRequest("Request body is required");
        }

        var command = new UpdateNoteCommand { Id = id, Dto = dto };
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
    /// Usuwa Note (soft delete).
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, [FromQuery] Guid deletedByUserId)
    {
        var command = new DeleteNoteCommand(id, deletedByUserId);
        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            if (result.Error.Contains("not found"))
            {
                return NotFound(result.Error);
            }
            return BadRequest(result.Error);
        }

        return NoContent();
    }
}
