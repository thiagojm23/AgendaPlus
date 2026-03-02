using AgendaPlus.Application.Commands.Users;
using FluentValidation;

namespace AgendaPlus.Application.Validators.Users;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("User ID is required");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MaximumLength(50).WithMessage("First name must be at most 50 characters");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(50).WithMessage("Last name must be at most 50 characters");

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(20).WithMessage("Phone number must be at most 20 characters")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber));

        RuleFor(x => x.Document)
            .MaximumLength(20).WithMessage("Document must be at most 20 characters")
            .When(x => !string.IsNullOrEmpty(x.Document));

        RuleFor(x => x.BusinessName)
            .MaximumLength(100).WithMessage("Business name must be at most 100 characters")
            .When(x => !string.IsNullOrEmpty(x.BusinessName));
    }
}