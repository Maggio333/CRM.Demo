using FluentValidation;

namespace CRM.Demo.Application.Tasks.Commands.UpdateTask;

/// <summary>
/// Walidator dla UpdateTaskCommand używając FluentValidation.
/// </summary>
public class UpdateTaskCommandValidator : AbstractValidator<UpdateTaskCommand>
{
    public UpdateTaskCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Task ID is required");
        
        RuleFor(x => x.Dto.Title)
            .NotEmpty().WithMessage("Task title is required")
            .MaximumLength(200).WithMessage("Task title must not exceed 200 characters");
        
        RuleFor(x => x.Dto.Type)
            .NotEmpty().WithMessage("Task type is required")
            .Must(type => new[] { "Call", "Meeting", "Email", "FollowUp", "Document", "Other" }.Contains(type))
            .WithMessage("Invalid task type");
        
        RuleFor(x => x.Dto.Priority)
            .NotEmpty().WithMessage("Task priority is required")
            .Must(priority => new[] { "Low", "Medium", "High", "Urgent" }.Contains(priority))
            .WithMessage("Invalid task priority");
        
        RuleFor(x => x.Dto.Description)
            .MaximumLength(5000).WithMessage("Description must not exceed 5000 characters")
            .When(x => !string.IsNullOrEmpty(x.Dto.Description));
        
        RuleFor(x => x.Dto.DueDate)
            .GreaterThan(DateTime.UtcNow).WithMessage("Due date must be in the future")
            .When(x => x.Dto.DueDate.HasValue);
        
        RuleFor(x => x.Dto)
            .Must(dto => !dto.DueDate.HasValue || !dto.StartDate.HasValue || dto.DueDate >= dto.StartDate)
            .WithMessage("Due date cannot be before start date");
    }
}
