using AgendaPlus.Application.Queries.Bookings;
using AgendaPlus.Application.QueryBuilders;
using AgendaPlus.Domain.Common;
using AgendaPlus.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AgendaPlus.Application.Handlers.Bookings;

public class SearchBookingByCodeQueryHandler(
    ILogger<SearchBookingByCodeQueryHandler> logger,
    BookingQueryBuilder bookingQueryBuilder) : IRequestHandler<SearchBookingByCodeQuery, Result<Booking>>
{
    public async Task<Result<Booking>> Handle(SearchBookingByCodeQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Searching booking by code: {Code}", request.ReservationCode);

        var now = DateTime.UtcNow;

        // Only return future bookings for privacy
        var booking = await bookingQueryBuilder
            .WithResource()
            .AsNoTracking()
            .Query
            .FirstOrDefaultAsync(b => b.ReservationCode == request.ReservationCode &&
                                      b.StartBookingDateTime > now,
                cancellationToken);

        if (booking == null)
        {
            logger.LogWarning("No future booking found with code: {Code}", request.ReservationCode);
            return Result.Failure<Booking>("Booking not found or already expired");
        }

        logger.LogInformation("Found booking with code: {Code}", request.ReservationCode);
        return Result.Success(booking);
    }
}