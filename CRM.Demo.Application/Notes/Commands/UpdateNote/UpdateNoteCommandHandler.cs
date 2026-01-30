using MediatR;
using CRM.Demo.Domain.Common;
using CRM.Demo.Domain.Notes.ValueObjects;
using CRM.Demo.Application.Notes.Interfaces;
using CRM.Demo.Application.Common.Interfaces;
using Unit = MediatR.Unit;

namespace CRM.Demo.Application.Notes.Commands.UpdateNote;

/// <summary>
/// Handler dla UpdateNoteCommand.
/// Aktualizuje Note.
/// </summary>
public class UpdateNoteCommandHandler : IRequestHandler<UpdateNoteCommand, Result<Unit, string>>
{
    private readonly INoteRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateNoteCommandHandler(
        INoteRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Unit, string>> Handle(
        UpdateNoteCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var note = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (note == null)
            {
                return Result<Unit, string>.Failure(
                    $"Note with ID {request.Id} not found"
                );
            }

            // Utwórz Value Objects
            var noteType = NoteType.FromString(request.Dto.Type);
            if (noteType == null)
            {
                return Result<Unit, string>.Failure("Invalid note type");
            }

            NoteCategory? category = null;
            if (!string.IsNullOrEmpty(request.Dto.Category))
            {
                category = NoteCategory.FromString(request.Dto.Category);
            }

            // Aktualizuj Note - użyj nowej metody Update
            // Używamy placeholder dla updatedByUserId - w produkcji z sesji użytkownika
            note.Update(
                request.Dto.Content,
                noteType,
                request.Dto.Title,
                category,
                request.Dto.CustomerId,
                request.Dto.ContactId,
                request.Dto.TaskId,
                Guid.Empty // Placeholder - w produkcji z sesji
            );

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Unit, string>.Success(Unit.Value);
        }
        catch (DomainException ex)
        {
            return Result<Unit, string>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            return Result<Unit, string>.Failure($"An error occurred: {ex.Message}");
        }
    }
}
