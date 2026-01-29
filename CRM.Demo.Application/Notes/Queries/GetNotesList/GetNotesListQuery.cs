using MediatR;
using CRM.Demo.Application.Notes.DTOs;

namespace CRM.Demo.Application.Notes.Queries.GetNotesList;

/// <summary>
/// Query dla pobrania listy Note z paginacją.
/// </summary>
public class GetNotesListQuery : IRequest<List<NoteDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public Guid? CustomerId { get; set; }
    public Guid? ContactId { get; set; }
    public Guid? TaskId { get; set; }
    public Guid? CreatedByUserId { get; set; }
}
