using AgendaPlus.Application.Queries.Bookings;
using AgendaPlus.Application.QueryBuilders;
using AgendaPlus.Domain.Common;
using AgendaPlus.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AgendaPlus.Application.Handlers.Bookings;

public class SearchBookingByEmailQueryHandler(
    ILogger<SearchBookingByEmailQueryHandler> logger,
    BookingQueryBuilder bookingQueryBuilder) : IRequestHandler<SearchBookingByEmailQuery, Result<List<Booking>>>
{
    public async Task<Result<List<Booking>>> Handle(SearchBookingByEmailQuery request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Searching bookings by email: {Email}", request.Email);

        var now = DateTime.UtcNow;

        // Buscar apenas bookings futuras para privacidade
        var bookings = await bookingQueryBuilder
            .WithResource()
            .AsNoTracking()
            .Query
            .Where(b => b.StartBookingDateTime > now)
            .ToListAsync(cancellationToken);

        // Filtrar no client-side devido ao JSONB CustomerData
        var matchingBookings = bookings
            .Where(b => b.CustomerData.Email != null &&
                        b.CustomerData.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase))
            .OrderBy(b => b.StartBookingDateTime)
            .ToList();

        logger.LogInformation("Found {Count} future bookings for email: {Email}",
            matchingBookings.Count, request.Email);

        return Result.Success(matchingBookings);
    }
}