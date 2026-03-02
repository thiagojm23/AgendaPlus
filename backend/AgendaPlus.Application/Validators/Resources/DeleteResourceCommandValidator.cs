using AgendaPlus.Application.Commands.Resources;
using FluentValidation;

namespace AgendaPlus.Application.Validators.Resources;

public class DeleteResourceCommandValidator : AbstractValidator<DeleteResourceCommand>
{
    public DeleteResourceCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Resource ID is required");
    }
}