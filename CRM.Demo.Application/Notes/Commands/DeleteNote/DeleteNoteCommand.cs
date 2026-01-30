using MediatR;
using CRM.Demo.Domain.Common;
using Unit = MediatR.Unit;

namespace CRM.Demo.Application.Notes.Commands.DeleteNote;

/// <summary>
/// Command dla usunięcia Note.
/// </summary>
public class DeleteNoteCommand : IRequest<Result<Unit, string>>
{
    public Guid NoteId { get; set; }
    public Guid DeletedByUserId { get; set; }

    public DeleteNoteCommand(Guid noteId, Guid deletedByUserId)
    {
        NoteId = noteId;
        DeletedByUserId = deletedByUserId;
    }
}
