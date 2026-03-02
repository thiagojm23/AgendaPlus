using AgendaPlus.Application.Commands.Resources;
using FluentValidation;

namespace AgendaPlus.Application.Validators.Resources;

public class UpdateResourceCommandValidator : AbstractValidator<UpdateResourceCommand>
{
    public UpdateResourceCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Resource ID is required");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Resource name is required")
            .MaximumLength(100).WithMessage("Name must be at most 100 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description must be at most 500 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.ResourceType)
            .IsInEnum().WithMessage("Resource type is invalid");

        RuleFor(x => x.OpenDays)
            .NotEmpty().WithMessage("Operating days are required");

        RuleFor(x => x.IsActive)
            .NotNull().WithMessage("Active/inactive status is required");
    }
}