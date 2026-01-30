using FluentValidation;

namespace CRM.Demo.Application.Notes.Commands.UpdateNote;

/// <summary>
/// Walidator dla UpdateNoteCommand używając FluentValidation.
/// </summary>
public class UpdateNoteCommandValidator : AbstractValidator<UpdateNoteCommand>
{
    public UpdateNoteCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Note ID is required");

        RuleFor(x => x.Dto.Content)
            .NotEmpty().WithMessage("Note content is required")
            .MaximumLength(10000).WithMessage("Note content must not exceed 10000 characters");

        RuleFor(x => x.Dto.Type)
            .NotEmpty().WithMessage("Note type is required")
            .Must(type => new[] { "General", "Call", "Meeting", "Email", "FollowUp", "Other" }.Contains(type))
            .WithMessage("Invalid note type");

        RuleFor(x => x.Dto.Title)
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters")
            .When(x => !string.IsNullOrEmpty(x.Dto.Title));

        RuleFor(x => x.Dto)
            .Must(dto => dto.CustomerId.HasValue || dto.ContactId.HasValue || dto.TaskId.HasValue)
            .WithMessage("Note must be linked to Customer, Contact, or Task");
    }
}
