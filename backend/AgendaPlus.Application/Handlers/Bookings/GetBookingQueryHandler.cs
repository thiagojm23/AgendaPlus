using AgendaPlus.Application.Queries.Bookings;
using AgendaPlus.Application.QueryBuilders;
using AgendaPlus.Domain.Common;
using AgendaPlus.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AgendaPlus.Application.Handlers.Bookings;

public class GetBookingQueryHandler(
    ILogger<GetBookingQueryHandler> logger,
    BookingQueryBuilder bookingQueryBuilder) : IRequestHandler<GetBookingQuery, Result<Booking>>
{
    public async Task<Result<Booking>> Handle(GetBookingQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting booking with ID: {Id}", request.Id);

        var booking = await bookingQueryBuilder
            .WithResource()
            .AsNoTracking()
            .Query
            .FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

        if (booking == null)
        {
            logger.LogWarning("Booking not found with ID: {Id}", request.Id);
            return Result.Failure<Booking>("Booking not found");
        }

        logger.LogInformation("Booking found with ID: {Id}", request.Id);
        return Result.Success(booking);
    }
}