using AgendaPlus.Application.Commands.AvailabilityExceptions;
using AgendaPlus.Domain.Enums;
using FluentValidation;

namespace AgendaPlus.Application.Validators.AvailabilityExceptions;

public class CreateAvailabilityExceptionCommandValidator : AbstractValidator<CreateAvailabilityExceptionCommand>
{
    public CreateAvailabilityExceptionCommandValidator()
    {
        RuleFor(x => x.ResourceId)
            .NotEmpty().WithMessage("Resource ID is required");

        RuleFor(x => x.Strategy)
            .IsInEnum().WithMessage("Availability exception strategy is invalid");

        RuleFor(x => x.StartBlockTime)
            .NotEmpty().WithMessage("Block start date/time is required");

        RuleFor(x => x.EndBlockTime)
            .NotEmpty().WithMessage("Block end date/time is required");

        RuleFor(x => x.EndBlockTime)
            .GreaterThan(x => x.StartBlockTime)
            .WithMessage("Block end date/time must be after block start date/time");

        RuleFor(x => x.Reason)
            .MaximumLength(500).WithMessage("Reason must be at most 500 characters")
            .When(x => !string.IsNullOrEmpty(x.Reason));

        RuleFor(x => x.OverrideStartTime)
            .NotEmpty().WithMessage("Override start time is required")
            .When(x => x.Strategy == StrategyAvailabilityException.OverrideTime ||
                       x.Strategy == StrategyAvailabilityException.OverrideAll);

        RuleFor(x => x.OverrideEndTime)
            .NotEmpty().WithMessage("Override end time is required")
            .When(x => x.Strategy == StrategyAvailabilityException.OverrideTime ||
                       x.Strategy == StrategyAvailabilityException.OverrideAll);

        RuleFor(x => x.OverrideEndTime)
            .GreaterThan(x => x.OverrideStartTime)
            .WithMessage("Override end time must be after override start time")
            .When(x => x.OverrideStartTime.HasValue && x.OverrideEndTime.HasValue);

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than zero")
            .When(x => (x.Price.HasValue && x.Strategy == StrategyAvailabilityException.OverridePrice) ||
                       (x.Price.HasValue && x.Strategy == StrategyAvailabilityException.OverrideAll))
            .Must(price => price == null || price % 0.01m == 0)
            .WithMessage("Price must have at most 2 decimal places")
            .When(x => x.Price.HasValue);
    }
}