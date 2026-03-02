using AgendaPlus.Application.Queries.Bookings;
using AgendaPlus.Application.QueryBuilders;
using AgendaPlus.Domain.Common;
using AgendaPlus.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AgendaPlus.Application.Handlers.Bookings;

public class ListBookingsQueryHandler(
    ILogger<ListBookingsQueryHandler> logger,
    BookingQueryBuilder bookingQueryBuilder) : IRequestHandler<ListBookingsQuery, Result<List<Booking>>>
{
    public async Task<Result<List<Booking>>> Handle(ListBookingsQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Listing bookings - Page: {Page}, PageSize: {PageSize}",
            request.PageNumber, request.PageSize);

        var query = bookingQueryBuilder
            .WithResource()
            .AsNoTracking()
            .Query;

        if (request.ResourceId.HasValue) query = query.Where(b => b.ResourceId == request.ResourceId.Value);

        if (request.StartDate.HasValue) query = query.Where(b => b.StartBookingDateTime >= request.StartDate.Value);

        if (request.EndDate.HasValue) query = query.Where(b => b.EndBookingDateTime <= request.EndDate.Value);

        if (request.Status.HasValue) query = query.Where(b => b.Status == request.Status.Value);

        var bookings = await query
            .OrderByDescending(b => b.StartBookingDateTime)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        logger.LogInformation("Found {Count} bookings", bookings.Count);
        return Result.Success(bookings);
    }
}