using MediatR;
using CRM.Demo.Domain.Common;
using CRM.Demo.Application.Notes.Interfaces;
using CRM.Demo.Application.Common.Interfaces;
using Unit = MediatR.Unit;

namespace CRM.Demo.Application.Notes.Commands.DeleteNote;

/// <summary>
/// Handler dla DeleteNoteCommand.
/// Usuwa Note z bazy danych (soft delete).
/// </summary>
public class DeleteNoteCommandHandler : IRequestHandler<DeleteNoteCommand, Result<Unit, string>>
{
    private readonly INoteRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    
    public DeleteNoteCommandHandler(
        INoteRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<Result<Unit, string>> Handle(
        DeleteNoteCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var note = await _repository.GetByIdAsync(request.NoteId, cancellationToken);
            
            if (note == null)
            {
                return Result<Unit, string>.Failure(
                    $"Note with ID {request.NoteId} not found"
                );
            }
            
            // Soft delete - publikuje Domain Event
            note.Delete(request.DeletedByUserId);
            
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            return Result<Unit, string>.Success(Unit.Value);
        }
        catch (Exception ex)
        {
            return Result<Unit, string>.Failure($"An error occurred: {ex.Message}");
        }
    }
}
