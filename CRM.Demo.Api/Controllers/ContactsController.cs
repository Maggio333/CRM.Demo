using Microsoft.AspNetCore.Mvc;
using MediatR;
using CRM.Demo.Application.Contacts.Commands.CreateContact;
using CRM.Demo.Application.Contacts.Commands.UpdateContact;
using CRM.Demo.Application.Contacts.Commands.DeleteContact;
using CRM.Demo.Application.Contacts.Queries.GetContactById;
using CRM.Demo.Application.Contacts.Queries.GetContactsList;
using CRM.Demo.Application.Contacts.DTOs;
using CRM.Demo.Domain.Common;

namespace CRM.Demo.Api.Controllers;

/// <summary>
/// Controller dla zarządzania Contacts.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ContactsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ContactsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Pobiera listę Contacts z filtrowaniem i paginacją.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<ContactDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetList(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        [FromQuery] Guid? customerId = null)
    {
        var query = new GetContactsListQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            SearchTerm = searchTerm,
            CustomerId = customerId
        };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Pobiera Contact po ID.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ContactDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var query = new GetContactByIdQuery(id);
        var result = await _mediator.Send(query);

        if (result.IsFailure)
        {
            return NotFound(result.Error);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Tworzy nowego Contact.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateContactDto dto)
    {
        if (dto == null)
        {
            return BadRequest("Request body is required");
        }

        var command = new CreateContactCommand { Dto = dto };
        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value);
    }

    /// <summary>
    /// Aktualizuje Contact.
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateContactDto dto)
    {
        if (dto == null)
        {
            return BadRequest("Request body is required");
        }

        var command = new UpdateContactCommand { Id = id, Dto = dto };
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
    /// Usuwa Contact.
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new DeleteContactCommand(id);
        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            return NotFound(result.Error);
        }

        return NoContent();
    }
}
