using MediatR;
using CRM.Demo.Domain.Common;
using CRM.Demo.Domain.Notes.Entities;
using CRM.Demo.Domain.Notes.ValueObjects;
using CRM.Demo.Application.Notes.Interfaces;
using CRM.Demo.Application.Common.Interfaces;

namespace CRM.Demo.Application.Notes.Commands.CreateNote;

/// <summary>
/// Handler dla CreateNoteCommand.
/// Tworzy nowy Note i zapisuje do bazy.
/// </summary>
public class CreateNoteCommandHandler : IRequestHandler<CreateNoteCommand, Result<Guid, string>>
{
    private readonly INoteRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateNoteCommandHandler(
        INoteRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid, string>> Handle(
        CreateNoteCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Utwórz Value Objects
            var noteType = NoteType.FromString(request.Dto.Type);

            NoteCategory? category = null;
            if (!string.IsNullOrEmpty(request.Dto.Category))
            {
                category = NoteCategory.FromString(request.Dto.Category);
            }

            // Utwórz Note (Entity)
            var note = Note.Create(
                request.Dto.Content,
                noteType,
                request.Dto.CreatedByUserId,
                request.Dto.Title,
                category,
                request.Dto.CustomerId,
                request.Dto.ContactId,
                request.Dto.TaskId
            );

            // Zapisz przez Repository
            await _repository.AddAsync(note, cancellationToken);

            // UnitOfWork zapisuje do bazy i publikuje Domain Events
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid, string>.Success(note.Id);
        }
        catch (DomainException ex)
        {
            return Result<Guid, string>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            return Result<Guid, string>.Failure($"An error occurred: {ex.Message}");
        }
    }
}
