using System.Text.Json;
using AgendaPlus.Application.Commands.Bookings;
using AgendaPlus.Application.Common.Interfaces;
using AgendaPlus.Application.Interfaces.Services;
using AgendaPlus.Application.QueryBuilders;
using AgendaPlus.Domain.Common;
using AgendaPlus.Domain.Entities;
using AgendaPlus.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AgendaPlus.Application.Handlers.Bookings;

public class CreateBookingLoggedCommandHandler(
    ILogger<CreateBookingLoggedCommandHandler> logger,
    IApplicationDbContext context,
    BookingQueryBuilder bookingQueryBuilder,
    UserQueryBuilder userQueryBuilder,
    ResourceQueryBuilder resourceQueryBuilder,
    ICurrentUserService currentUserService) : IRequestHandler<CreateBookingLoggedCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateBookingLoggedCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating logged booking for Resource {ResourceId}", request.ResourceId);

        // Idempotency check
        if (!string.IsNullOrEmpty(request.IdempotencyKey))
        {
            var existingBooking = await bookingQueryBuilder
                .AsNoTracking()
                .Query
                .FirstOrDefaultAsync(b => b.ReservationCode == request.IdempotencyKey, cancellationToken);

            if (existingBooking != null)
            {
                logger.LogInformation("Idempotent request - returning existing booking {Id}", existingBooking.Id);
                return Result.Success(existingBooking.Id);
            }
        }

        // Fetch user
        var user = await userQueryBuilder
            .AsNoTracking()
            .GetByIdAsync(currentUserService.UserId, cancellationToken);

        if (user == null) return Result.Failure<Guid>("User not found");

        // Fetch resource and tenant
        var resource = await resourceQueryBuilder
            .Query
            .FirstOrDefaultAsync(r => r.Id == request.ResourceId, cancellationToken);

        if (resource == null || !resource.IsActive) return Result.Failure<Guid>("Resource not found or inactive");

        // Check availability with row locking (concurrency)
        var hasConflict = await bookingQueryBuilder
            .Query
            .Where(b => b.ResourceId == request.ResourceId &&
                        (b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.Pending) &&
                        b.StartBookingDateTime < request.EndDateTime &&
                        b.EndBookingDateTime > request.StartDateTime)
            .AnyAsync(cancellationToken);

        if (hasConflict)
        {
            logger.LogWarning("Booking conflict detected for Resource {ResourceId}", request.ResourceId);
            return Result.Failure<Guid>("Time slot is already booked by another user");
        }

        // Calculate price based on patterns or exceptions
        var totalHours = (decimal)(request.EndDateTime - request.StartDateTime).TotalHours;
        decimal pricePerHour = 0;

        // Check if there is an exception with price override
        var exception = await context.AvailabilityExceptions
            .FirstOrDefaultAsync(e => e.ResourceId == request.ResourceId &&
                                      e.StartBlockTime <= request.StartDateTime &&
                                      e.EndBlockTime >= request.EndDateTime &&
                                      e.Price.HasValue,
                cancellationToken);

        if (exception != null && exception.Price.HasValue)
        {
            pricePerHour = exception.Price.Value;
        }
        else
        {
            // Fetch pattern for the day
            var dayOfWeek = request.StartDateTime.DayOfWeek;
            var dayFlag = (DaysOfWeekBitwise)(1 << (int)dayOfWeek);

            var pattern = await context.AvailabilityPatterns
                .FirstOrDefaultAsync(p => p.ResourceId == request.ResourceId &&
                                          p.DayOfWeek.HasFlag(dayFlag),
                    cancellationToken);

            if (pattern != null) pricePerHour = pattern.PricePerHour;
        }

        var totalPrice = pricePerHour * totalHours;

        // Generate unique reservation code
        var reservationCode = !string.IsNullOrEmpty(request.IdempotencyKey)
            ? request.IdempotencyKey
            : Guid.NewGuid().ToString("N")[..12].ToUpper();

        // Create booking
        var booking = new Booking
        {
            ResourceId = request.ResourceId,
            TenantId = resource.TenantId,
            UserId = currentUserService.UserId,
            StartBookingDateTime = request.StartDateTime,
            EndBookingDateTime = request.EndDateTime.AddMinutes(-1),
            ReservationCode = reservationCode,
            TotalPrice = totalPrice,
            Status = BookingStatus.Confirmed,
            CustomerData = new CustomerData(
                $"{user.FirstName} {user.SecondName}",
                user.Email,
                user.PhoneNumber)
            {
                Name = $"{user.FirstName} {user.SecondName}"
            }
        };

        context.Bookings.Add(booking);
        await context.SaveChangesAsync(cancellationToken);

        // Add Outbox message for email sending
        var outboxMessage = new OutboxMessage
        {
            Type = OutboxMessageType.BookingCreated,
            Content = JsonDocument.Parse(JsonSerializer.Serialize(new
            {
                BookingId = booking.Id,
                CustomerEmail = user.Email,
                CustomerName = $"{user.FirstName} {user.SecondName}",
                request.ResourceId,
                ReservationCode = reservationCode,
                request.StartDateTime,
                request.EndDateTime,
                TotalPrice = totalPrice
            })),
            Status = OutboxStatus.Pending,
            OccurredOn = DateTimeOffset.UtcNow
        };

        context.OutboxMessages.Add(outboxMessage);
        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Booking created successfully with ID: {Id}, Code: {Code}",
            booking.Id, reservationCode);

        return Result.Success(booking.Id);
    }
}