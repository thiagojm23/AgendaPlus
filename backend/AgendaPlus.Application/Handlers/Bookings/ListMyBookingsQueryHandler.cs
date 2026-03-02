using AgendaPlus.Application.Interfaces.Services;
using AgendaPlus.Application.Queries.Bookings;
using AgendaPlus.Application.QueryBuilders;
using AgendaPlus.Domain.Common;
using AgendaPlus.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AgendaPlus.Application.Handlers.Bookings;

public class ListMyBookingsQueryHandler(
    ILogger<ListMyBookingsQueryHandler> logger,
    BookingQueryBuilder bookingQueryBuilder,
    ICurrentUserService currentUserService) : IRequestHandler<ListMyBookingsQuery, Result<List<Booking>>>
{
    public async Task<Result<List<Booking>>> Handle(ListMyBookingsQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId;
        logger.LogInformation("Listing bookings for user {UserId}", userId);

        var query = bookingQueryBuilder
            .WithResource()
            .AsNoTracking()
            .Query
            .Where(b => b.UserId == userId);

        if (request.Status.HasValue) query = query.Where(b => b.Status == request.Status.Value);

        var bookings = await query
            .OrderByDescending(b => b.StartBookingDateTime)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        logger.LogInformation("Found {Count} bookings for user {UserId}", bookings.Count, userId);
        return Result.Success(bookings);
    }
}