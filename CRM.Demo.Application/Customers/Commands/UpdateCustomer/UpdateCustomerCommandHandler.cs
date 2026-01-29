using MediatR;
using CRM.Demo.Domain.Common;
using CRM.Demo.Domain.Customers.ValueObjects;
using CRM.Demo.Application.Customers.Interfaces;
using CRM.Demo.Application.Common.Interfaces;
using System.Linq;
using Unit = MediatR.Unit;

namespace CRM.Demo.Application.Customers.Commands.UpdateCustomer;

/// <summary>
/// Handler dla UpdateCustomerCommand.
/// Aktualizuje dane kontaktowe Customer.
/// </summary>
public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, Result<Unit, string>>
{
    private readonly ICustomerRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    
    public UpdateCustomerCommandHandler(
        ICustomerRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<Result<Unit, string>> Handle(
        UpdateCustomerCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var customer = await _repository.GetByIdAsync(request.CustomerId, cancellationToken);
            
            if (customer == null)
            {
                return Result<Unit, string>.Failure(
                    $"Customer with ID {request.CustomerId} not found"
                );
            }
            
            // Utwórz Value Objects
            var email = Email.Create(request.Email);
            PhoneNumber? phoneNumber = null;
            if (!string.IsNullOrEmpty(request.PhoneNumber))
            {
                // Normalizuj numer telefonu - usuń kierunkowy +48 jeśli jest
                var normalizedPhone = NormalizePhoneNumber(request.PhoneNumber);
                if (normalizedPhone == null)
                {
                    return Result<Unit, string>.Failure("Invalid phone number format. Must be 9 digits (with or without +48)");
                }
                
                phoneNumber = PhoneNumber.Create("48", normalizedPhone);
            }
            
            Address? address = null;
            if (!string.IsNullOrEmpty(request.Street) &&
                !string.IsNullOrEmpty(request.City) &&
                !string.IsNullOrEmpty(request.PostalCode) &&
                !string.IsNullOrEmpty(request.Country))
            {
                address = Address.Create(
                    request.Street,
                    request.City,
                    request.PostalCode,
                    request.Country
                );
            }
            
            // Aktualizuj Customer (metoda biznesowa)
            customer.UpdateContactInfo(email, phoneNumber, address);
            
            // Zapisuj zmiany
            await _repository.UpdateAsync(customer, cancellationToken);
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
