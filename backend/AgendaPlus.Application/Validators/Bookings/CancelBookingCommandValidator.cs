using AgendaPlus.Application.Commands.Bookings;
using FluentValidation;

namespace AgendaPlus.Application.Validators.Bookings;

public class CancelBookingCommandValidator : AbstractValidator<CancelBookingCommand>
{
    public CancelBookingCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Booking ID is required");
    }
}