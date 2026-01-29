using MediatR;
using AutoMapper;
using CRM.Demo.Domain.Common;
using CRM.Demo.Application.Notes.DTOs;
using CRM.Demo.Application.Notes.Interfaces;

namespace CRM.Demo.Application.Notes.Queries.GetNoteById;

/// <summary>
/// Handler dla GetNoteByIdQuery.
/// Pobiera Note po ID i mapuje na DTO.
/// </summary>
public class GetNoteByIdQueryHandler : IRequestHandler<GetNoteByIdQuery, Result<NoteDto, string>>
{
    private readonly INoteRepository _repository;
    private readonly IMapper _mapper;
    
    public GetNoteByIdQueryHandler(
        INoteRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }
    
    public async Task<Result<NoteDto, string>> Handle(
        GetNoteByIdQuery request,
        CancellationToken cancellationToken)
    {
        var note = await _repository.GetByIdAsync(request.NoteId, cancellationToken);
        
        if (note == null)
        {
            return Result<NoteDto, string>.Failure(
                $"Note with ID {request.NoteId} not found"
            );
        }
        
        var noteDto = _mapper.Map<NoteDto>(note);
        return Result<NoteDto, string>.Success(noteDto);
    }
}
