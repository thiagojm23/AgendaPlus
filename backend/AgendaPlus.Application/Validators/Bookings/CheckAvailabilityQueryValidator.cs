using AgendaPlus.Application.Queries.Bookings;
using FluentValidation;

namespace AgendaPlus.Application.Validators.Bookings;

public class CheckAvailabilityQueryValidator : AbstractValidator<CheckAvailabilityQuery>
{
    public CheckAvailabilityQueryValidator()
    {
        RuleFor(x => x.ResourceId)
            .NotEmpty().WithMessage("Resource ID is required");

        RuleFor(x => x.StartDateTime)
            .NotEmpty().WithMessage("Start date/time is required");

        RuleFor(x => x.EndDateTime)
            .NotEmpty().WithMessage("End date/time is required")
            .GreaterThan(x => x.StartDateTime).WithMessage("End date/time must be after start date/time");
    }
}