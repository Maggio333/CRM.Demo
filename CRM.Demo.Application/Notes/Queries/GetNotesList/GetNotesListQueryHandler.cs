using MediatR;
using AutoMapper;
using CRM.Demo.Application.Notes.DTOs;
using CRM.Demo.Application.Notes.Interfaces;

namespace CRM.Demo.Application.Notes.Queries.GetNotesList;

/// <summary>
/// Handler dla GetNotesListQuery.
/// Pobiera listę Note z paginacją i mapuje na DTOs.
/// </summary>
public class GetNotesListQueryHandler : IRequestHandler<GetNotesListQuery, List<NoteDto>>
{
    private readonly INoteRepository _repository;
    private readonly IMapper _mapper;

    public GetNotesListQueryHandler(
        INoteRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<List<NoteDto>> Handle(
        GetNotesListQuery request,
        CancellationToken cancellationToken)
    {
        List<Domain.Notes.Entities.Note> notes;

        if (request.CustomerId.HasValue)
        {
            notes = await _repository.GetByCustomerIdAsync(request.CustomerId.Value, cancellationToken);
        }
        else if (request.ContactId.HasValue)
        {
            notes = await _repository.GetByContactIdAsync(request.ContactId.Value, cancellationToken);
        }
        else if (request.TaskId.HasValue)
        {
            notes = await _repository.GetByTaskIdAsync(request.TaskId.Value, cancellationToken);
        }
        else if (request.CreatedByUserId.HasValue)
        {
            notes = await _repository.GetByCreatedByUserIdAsync(request.CreatedByUserId.Value, cancellationToken);
        }
        else
        {
            notes = await _repository.GetAllAsync(cancellationToken);
        }

        // Paginacja
        var paginatedNotes = notes
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        return _mapper.Map<List<NoteDto>>(paginatedNotes);
    }
}
