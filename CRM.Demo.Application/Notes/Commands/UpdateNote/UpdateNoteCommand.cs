using MediatR;
using CRM.Demo.Application.Notes.DTOs;
using CRM.Demo.Domain.Common;
using Unit = MediatR.Unit;

namespace CRM.Demo.Application.Notes.Commands.UpdateNote;

/// <summary>
/// Command dla aktualizacji Note.
/// </summary>
public class UpdateNoteCommand : IRequest<Result<Unit, string>>
{
    public Guid Id { get; set; }
    public UpdateNoteDto Dto { get; set; } = null!;
}
