using MediatR;
using CRM.Demo.Application.Notes.DTOs;
using CRM.Demo.Domain.Common;

namespace CRM.Demo.Application.Notes.Queries.GetNoteById;

/// <summary>
/// Query dla pobrania Note po ID.
/// </summary>
public class GetNoteByIdQuery : IRequest<Result<NoteDto, string>>
{
    public Guid NoteId { get; set; }
    
    public GetNoteByIdQuery(Guid noteId)
    {
        NoteId = noteId;
    }
}
