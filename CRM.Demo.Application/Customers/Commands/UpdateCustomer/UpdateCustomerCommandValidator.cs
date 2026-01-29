using FluentValidation;
using System.Linq;

namespace CRM.Demo.Application.Customers.Commands.UpdateCustomer;

/// <summary>
/// Walidator dla UpdateCustomerCommand.
/// </summary>
public class UpdateCustomerCommandValidator : AbstractValidator<UpdateCustomerCommand>
{
    public UpdateCustomerCommandValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty().WithMessage("Customer ID is required");
        
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(255).WithMessage("Email must not exceed 255 characters");
        
        RuleFor(x => x.PhoneNumber)
            .Must(phone => IsValidPhoneNumber(phone))
            .WithMessage("Phone number must be 9 digits (with or without country code +48)")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber));
        
        // Helper method do walidacji numeru telefonu
        // Akceptuje: 9 cyfr, +489 cyfr, 489 cyfr, +48 9 cyfr (ze spacją)
    }
    
    private static bool IsValidPhoneNumber(string? phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return true; // Opcjonalne pole
        
        // Usuń wszystkie znaki niebędące cyframi
        var digitsOnly = new string(phoneNumber.Where(char.IsDigit).ToArray());
        
        // Sprawdź czy ma 9 cyfr (bez kierunkowego) lub 11 cyfr (z kierunkowym 48)
        return digitsOnly.Length == 9 || (digitsOnly.Length == 11 && digitsOnly.StartsWith("48"));
    }
}
