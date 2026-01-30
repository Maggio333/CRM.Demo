using MediatR;
using CRM.Demo.Domain.Common;
using CRM.Demo.Domain.Contacts.ValueObjects;
using CRM.Demo.Application.Contacts.Interfaces;
using CRM.Demo.Application.Common.Interfaces;
using System.Linq;
using Unit = MediatR.Unit;

namespace CRM.Demo.Application.Contacts.Commands.UpdateContact;

/// <summary>
/// Handler dla UpdateContactCommand.
/// Aktualizuje Contact.
/// </summary>
public class UpdateContactCommandHandler : IRequestHandler<UpdateContactCommand, Result<Unit, string>>
{
    private readonly IContactRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateContactCommandHandler(
        IContactRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Unit, string>> Handle(
        UpdateContactCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var contact = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (contact == null)
            {
                return Result<Unit, string>.Failure(
                    $"Contact with ID {request.Id} not found"
                );
            }

            // Walidacja podstawowa
            if (string.IsNullOrWhiteSpace(request.Dto.FirstName))
            {
                return Result<Unit, string>.Failure("First name is required");
            }

            if (string.IsNullOrWhiteSpace(request.Dto.LastName))
            {
                return Result<Unit, string>.Failure("Last name is required");
            }

            // Utwórz Value Objects
            var emailResult = Email.Create(request.Dto.Email);
            if (emailResult == null)
            {
                return Result<Unit, string>.Failure("Invalid email format");
            }

            var contactType = ContactType.FromString(request.Dto.Type);
            if (contactType == null)
            {
                return Result<Unit, string>.Failure($"Invalid contact type: {request.Dto.Type}. Must be 'Person' or 'Company'");
            }

            PhoneNumber? phoneNumber = null;
            if (!string.IsNullOrEmpty(request.Dto.PhoneNumber))
            {
                // Normalizuj numer telefonu - usuń kierunkowy +48 jeśli jest
                var normalizedPhone = NormalizePhoneNumber(request.Dto.PhoneNumber);
                if (normalizedPhone == null)
                {
                    return Result<Unit, string>.Failure("Invalid phone number format. Must be 9 digits (with or without +48)");
                }

                phoneNumber = PhoneNumber.Create("48", normalizedPhone);
            }

            // Aktualizuj Contact - użyj nowej metody Update
            contact.Update(
                request.Dto.FirstName,
                request.Dto.LastName,
                emailResult,
                contactType,
                phoneNumber,
                request.Dto.JobTitle,
                request.Dto.Department,
                request.Dto.CustomerId
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
            return Result<Unit, string>.Failure($"An error occurred: {ex.Message}. Inner: {ex.InnerException?.Message}");
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
