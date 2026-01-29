using MediatR;
using CRM.Demo.Domain.Common;
using CRM.Demo.Application.Contacts.Interfaces;
using CRM.Demo.Application.Common.Interfaces;
using Unit = MediatR.Unit;

namespace CRM.Demo.Application.Contacts.Commands.DeleteContact;

/// <summary>
/// Handler dla DeleteContactCommand.
/// Usuwa Contact z bazy danych.
/// </summary>
public class DeleteContactCommandHandler : IRequestHandler<DeleteContactCommand, Result<Unit, string>>
{
    private readonly IContactRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    
    public DeleteContactCommandHandler(
        IContactRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<Result<Unit, string>> Handle(
        DeleteContactCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var contact = await _repository.GetByIdAsync(request.ContactId, cancellationToken);
            
            if (contact == null)
            {
                return Result<Unit, string>.Failure(
                    $"Contact with ID {request.ContactId} not found"
                );
            }
            
            // Usuń Contact
            await _repository.DeleteAsync(contact, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            return Result<Unit, string>.Success(Unit.Value);
        }
        catch (Exception ex)
        {
            return Result<Unit, string>.Failure($"An error occurred: {ex.Message}");
        }
    }
}
