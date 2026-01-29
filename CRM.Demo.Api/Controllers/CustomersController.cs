using Microsoft.AspNetCore.Mvc;
using MediatR;
using CRM.Demo.Application.Customers.Commands.CreateCustomer;
using CRM.Demo.Application.Customers.Commands.UpdateCustomer;
using CRM.Demo.Application.Customers.Commands.DeleteCustomer;
using CRM.Demo.Application.Customers.Queries.GetCustomerById;
using CRM.Demo.Application.Customers.Queries.GetCustomersList;
using CRM.Demo.Application.Customers.DTOs;
using CRM.Demo.Domain.Common;

namespace CRM.Demo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly IMediator _mediator;

    public CustomersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Pobiera listę Customer z paginacją.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<CustomerDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetList(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null)
    {
        var query = new GetCustomersListQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            SearchTerm = searchTerm
        };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Pobiera Customer po ID.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var query = new GetCustomerByIdQuery(id);
        var result = await _mediator.Send(query);

        if (result.IsFailure)
        {
            return NotFound(result.Error);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Tworzy nowego Customer.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateCustomerCommand command)
    {
        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value);
    }

    /// <summary>
    /// Aktualizuje Customer.
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCustomerDto dto)
    {
        var command = new UpdateCustomerCommand
        {
            CustomerId = id,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            Street = dto.Street,
            City = dto.City,
            PostalCode = dto.PostalCode,
            Country = dto.Country
        };
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

    /// <summary>
    /// Usuwa Customer.
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new DeleteCustomerCommand(id);
        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            return NotFound(result.Error);
        }

        return NoContent();
    }
}
