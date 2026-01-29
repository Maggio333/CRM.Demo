using MediatR;
using CRM.Demo.Domain.Common;
using CRM.Demo.Domain.Contacts.Entities;
using CRM.Demo.Domain.Contacts.ValueObjects;
using CRM.Demo.Application.Contacts.Interfaces;
using CRM.Demo.Application.Common.Interfaces;
using System.Linq;

namespace CRM.Demo.Application.Contacts.Commands.CreateContact;

/// <summary>
/// Handler dla CreateContactCommand.
/// Tworzy nowego Contact i zapisuje do bazy.
/// </summary>
public class CreateContactCommandHandler : IRequestHandler<CreateContactCommand, Result<Guid, string>>
{
    private readonly IContactRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    
    public CreateContactCommandHandler(
        IContactRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<Result<Guid, string>> Handle(
        CreateContactCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Sprawdź czy Contact z tym emailem już istnieje
            var existingContact = await _repository.FindByEmailAsync(request.Dto.Email, cancellationToken);
            if (existingContact != null)
            {
                return Result<Guid, string>.Failure(
                    $"Contact with email {request.Dto.Email} already exists"
                );
            }
            
            // Utwórz Value Objects
            var email = Email.Create(request.Dto.Email);
            var contactType = ContactType.FromString(request.Dto.Type);
            
            PhoneNumber? phoneNumber = null;
            if (!string.IsNullOrEmpty(request.Dto.PhoneNumber))
            {
                // Normalizuj numer telefonu - usuń kierunkowy +48 jeśli jest
                var normalizedPhone = NormalizePhoneNumber(request.Dto.PhoneNumber);
                if (normalizedPhone == null)
                {
                    return Result<Guid, string>.Failure("Invalid phone number format. Must be 9 digits (with or without +48)");
                }
                
                phoneNumber = PhoneNumber.Create("48", normalizedPhone);
            }
            
            // Utwórz Contact (Entity)
            var contact = Contact.Create(
                request.Dto.FirstName,
                request.Dto.LastName,
                email,
                contactType,
                phoneNumber,
                request.Dto.JobTitle,
                request.Dto.Department,
                request.Dto.CustomerId
            );
            
            // Zapisz przez Repository
            await _repository.AddAsync(contact, cancellationToken);
            
            // UnitOfWork zapisuje do bazy i publikuje Domain Events
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            return Result<Guid, string>.Success(contact.Id);
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
    
    /// <summary>
    /// Normalizuje numer telefonu - usuwa kierunkowy +48 i zwraca tylko 9 cyfr.
    /// </summary>
    private static string? NormalizePhoneNumber(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return null;
        
        // Usuń wszystkie znaki niebędące cyframi
        var digitsOnly = new string(phoneNumber.Where(char.IsDigit).ToArray());
        
        // Jeśli ma 11 cyfr i zaczyna się od 48, usuń kierunkowy
        if (digitsOnly.Length == 11 && digitsOnly.StartsWith("48"))
        {
            return digitsOnly.Substring(2); // Zwróć ostatnie 9 cyfr
        }
        
        // Jeśli ma 9 cyfr, zwróć jak jest
        if (digitsOnly.Length == 9)
        {
            return digitsOnly;
        }
        
        // Nieprawidłowy format
        return null;
    }
}
