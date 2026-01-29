using MediatR;
using CRM.Demo.Application.Notes.DTOs;
using CRM.Demo.Domain.Common;

namespace CRM.Demo.Application.Notes.Commands.CreateNote;

/// <summary>
/// Command dla tworzenia nowego Note.
/// </summary>
public class CreateNoteCommand : IRequest<Result<Guid, string>>
{
    public CreateNoteDto Dto { get; set; } = null!;
}
