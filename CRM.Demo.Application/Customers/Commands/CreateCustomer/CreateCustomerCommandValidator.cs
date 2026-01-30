using FluentValidation;
using System.Linq;

namespace CRM.Demo.Application.Customers.Commands.CreateCustomer;

/// <summary>
/// Walidator dla CreateCustomerCommand używając FluentValidation.
/// </summary>
public class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerCommandValidator()
    {
        RuleFor(x => x.CompanyName)
            .NotEmpty().WithMessage("Company name is required")
            .MaximumLength(200).WithMessage("Company name must not exceed 200 characters");

        RuleFor(x => x.TaxId)
            .NotEmpty().WithMessage("Tax ID is required")
            .Length(10).WithMessage("Tax ID must be 10 characters");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(255).WithMessage("Email must not exceed 255 characters");

        RuleFor(x => x.PhoneNumber)
            .Must(phone => IsValidPhoneNumber(phone))
            .WithMessage("Phone number must be 9 digits (with or without country code +48)")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber));

        RuleFor(x => x.Street)
            .MaximumLength(200).WithMessage("Street must not exceed 200 characters")
            .When(x => !string.IsNullOrEmpty(x.Street));

        RuleFor(x => x.City)
            .MaximumLength(100).WithMessage("City must not exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.City));

        RuleFor(x => x.PostalCode)
            .MaximumLength(20).WithMessage("Postal code must not exceed 20 characters")
            .When(x => !string.IsNullOrEmpty(x.PostalCode));

        RuleFor(x => x.Country)
            .MaximumLength(100).WithMessage("Country must not exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.Country));
    }

    // Helper method do walidacji numeru telefonu
    // Akceptuje: 9 cyfr, +489 cyfr, 489 cyfr, +48 9 cyfr (ze spacją)
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
