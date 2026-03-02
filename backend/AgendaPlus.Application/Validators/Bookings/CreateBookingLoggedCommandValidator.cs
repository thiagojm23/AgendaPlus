using AgendaPlus.Application.Commands.Bookings;
using FluentValidation;

namespace AgendaPlus.Application.Validators.Bookings;

public class CreateBookingLoggedCommandValidator : AbstractValidator<CreateBookingLoggedCommand>
{
    public CreateBookingLoggedCommandValidator()
    {
        RuleFor(x => x.ResourceId)
            .NotEmpty().WithMessage("Resource ID is required");

        RuleFor(x => x.StartDateTime)
            .NotEmpty().WithMessage("Start date/time is required")
            .Must(BeInFuture).WithMessage("The booking must be for a future date");

        RuleFor(x => x.EndDateTime)
            .NotEmpty().WithMessage("End date/time is required")
            .GreaterThan(x => x.StartDateTime).WithMessage("End date/time must be after start date/time");

        RuleFor(x => x)
            .Must(x => (x.EndDateTime - x.StartDateTime).TotalMinutes >= 30)
            .WithMessage("Booking must have a minimum duration of 30 minutes");
    }

    private bool BeInFuture(DateTime dateTime)
    {
        return dateTime > DateTime.UtcNow;
    }
}