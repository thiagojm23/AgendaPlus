using AgendaPlus.Application.Commands.Bookings;
using FluentValidation;

namespace AgendaPlus.Application.Validators.Bookings;

public class CreateBookingGuestCommandValidator : AbstractValidator<CreateBookingGuestCommand>
{
    public CreateBookingGuestCommandValidator()
    {
        RuleFor(x => x.ResourceId)
            .NotEmpty().WithMessage("Resource ID is required");

        RuleFor(x => x.TenantId)
            .NotEmpty().WithMessage("Establishment ID is required");

        RuleFor(x => x.CustomerName)
            .NotEmpty().WithMessage("Customer name is required")
            .MaximumLength(100).WithMessage("Name must be at most 100 characters");

        RuleFor(x => x)
            .Must(x => !string.IsNullOrEmpty(x.CustomerEmail) || !string.IsNullOrEmpty(x.CustomerPhone))
            .WithMessage("Email or phone number is required for contact");

        RuleFor(x => x.CustomerEmail)
            .EmailAddress().WithMessage("Invalid email")
            .When(x => !string.IsNullOrEmpty(x.CustomerEmail));

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