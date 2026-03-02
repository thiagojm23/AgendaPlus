using AgendaPlus.Application.Commands.Bookings;
using AgendaPlus.Application.Common.Interfaces;
using AgendaPlus.Application.QueryBuilders;
using AgendaPlus.Domain.Common;
using AgendaPlus.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AgendaPlus.Application.Handlers.Bookings;

public class CancelBookingCommandHandler(
    ILogger<CancelBookingCommandHandler> logger,
    BookingQueryBuilder bookingQueryBuilder,
    IApplicationDbContext context) : IRequestHandler<CancelBookingCommand, Result>
{
    public async Task<Result> Handle(CancelBookingCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Cancelling booking with ID: {Id}", request.Id);

        var booking = await bookingQueryBuilder
            .GetByIdAsync(request.Id, cancellationToken);

        if (booking == null)
        {
            logger.LogWarning("Booking not found with ID: {Id}", request.Id);
            return Result.Failure("Booking not found");
        }

        if (booking.Status == BookingStatus.Cancelled)
        {
            logger.LogWarning("Booking already cancelled: {Id}", request.Id);
            return Result.Failure("Booking has already been cancelled");
        }

        booking.Status = BookingStatus.Cancelled;
        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Booking cancelled successfully: {Id}", request.Id);
        return Result.Success();
    }
}