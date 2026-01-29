using MediatR;
using CRM.Demo.Domain.Common;
using CRM.Demo.Domain.Customers.Entities;
using CRM.Demo.Domain.Customers.ValueObjects;
using CRM.Demo.Application.Customers.Interfaces;
using CRM.Demo.Application.Common.Interfaces;
using System.Linq;

namespace CRM.Demo.Application.Customers.Commands.CreateCustomer;

/// <summary>
/// Handler dla CreateCustomerCommand.
/// Tworzy nowego Customer i zapisuje do bazy.
/// </summary>
public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, Result<Guid, string>>
{
    private readonly ICustomerRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    
    public CreateCustomerCommandHandler(
        ICustomerRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<Result<Guid, string>> Handle(
        CreateCustomerCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Sprawdź czy Customer z tym emailem już istnieje
            var existingCustomer = await _repository.FindByEmailAsync(request.Email, cancellationToken);
            if (existingCustomer != null)
            {
                return Result<Guid, string>.Failure(
                    $"Customer with email {request.Email} already exists"
                );
            }
            
            // Sprawdź czy Customer z tym TaxId już istnieje
            var existingByTaxId = await _repository.FindByTaxIdAsync(request.TaxId, cancellationToken);
            if (existingByTaxId != null)
            {
                return Result<Guid, string>.Failure(
                    $"Customer with Tax ID {request.TaxId} already exists"
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
                    return Result<Guid, string>.Failure("Invalid phone number format. Must be 9 digits (with or without +48)");
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
            
            // Utwórz Customer (Entity)
            var customer = Customer.Create(
                request.CompanyName,
                request.TaxId,
                email,
                phoneNumber,
                address
            );
            
            // Zapisz przez Repository
            await _repository.AddAsync(customer, cancellationToken);
            
            // UnitOfWork zapisuje do bazy i publikuje Domain Events
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            return Result<Guid, string>.Success(customer.Id);
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
