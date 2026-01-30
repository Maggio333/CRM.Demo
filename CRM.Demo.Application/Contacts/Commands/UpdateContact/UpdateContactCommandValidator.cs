using FluentValidation;
using System.Linq;

namespace CRM.Demo.Application.Contacts.Commands.UpdateContact;

/// <summary>
/// Walidator dla UpdateContactCommand używając FluentValidation.
/// </summary>
public class UpdateContactCommandValidator : AbstractValidator<UpdateContactCommand>
{
    public UpdateContactCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Contact ID is required");

        RuleFor(x => x.Dto.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MaximumLength(100).WithMessage("First name must not exceed 100 characters");

        RuleFor(x => x.Dto.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(100).WithMessage("Last name must not exceed 100 characters");

        RuleFor(x => x.Dto.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(255).WithMessage("Email must not exceed 255 characters");

        RuleFor(x => x.Dto.Type)
            .NotEmpty().WithMessage("Contact type is required")
            .Must(type => type == "Person" || type == "Company")
            .WithMessage("Contact type must be 'Person' or 'Company'");

        RuleFor(x => x.Dto.PhoneNumber)
            .Must(phone => IsValidPhoneNumber(phone))
            .WithMessage("Phone number must be 9 digits (with or without country code +48)")
            .When(x => !string.IsNullOrEmpty(x.Dto.PhoneNumber));

        RuleFor(x => x.Dto.JobTitle)
            .MaximumLength(200).WithMessage("Job title must not exceed 200 characters")
            .When(x => !string.IsNullOrEmpty(x.Dto.JobTitle));

        RuleFor(x => x.Dto.Department)
            .MaximumLength(100).WithMessage("Department must not exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.Dto.Department));
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
