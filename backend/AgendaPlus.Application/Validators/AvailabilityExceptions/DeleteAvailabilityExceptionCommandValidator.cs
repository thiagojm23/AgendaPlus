using AgendaPlus.Application.Commands.AvailabilityExceptions;
using FluentValidation;

namespace AgendaPlus.Application.Validators.AvailabilityExceptions;

public class DeleteAvailabilityExceptionCommandValidator : AbstractValidator<DeleteAvailabilityExceptionCommand>
{
    public DeleteAvailabilityExceptionCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Availability exception ID is required");
    }
}