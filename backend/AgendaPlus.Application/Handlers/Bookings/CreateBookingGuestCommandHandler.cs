using System.Text.Json;
using AgendaPlus.Application.Commands.Bookings;
using AgendaPlus.Application.Common.Interfaces;
using AgendaPlus.Application.QueryBuilders;
using AgendaPlus.Domain.Common;
using AgendaPlus.Domain.Entities;
using AgendaPlus.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AgendaPlus.Application.Handlers.Bookings;

public class CreateBookingGuestCommandHandler(
    ILogger<CreateBookingGuestCommandHandler> logger,
    IApplicationDbContext context,
    BookingQueryBuilder bookingQueryBuilder,
    ResourceQueryBuilder resourceQueryBuilder) : IRequestHandler<CreateBookingGuestCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateBookingGuestCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating guest booking for Resource {ResourceId}", request.ResourceId);

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

        // Validate that email OR phone is provided
        if (string.IsNullOrEmpty(request.CustomerEmail) && string.IsNullOrEmpty(request.CustomerPhone))
            return Result.Failure<Guid>("Email or phone number is required");

        // Buscar recurso
        var resource = await resourceQueryBuilder
            .AsNoTracking()
            .Query
            .FirstOrDefaultAsync(r => r.Id == request.ResourceId && r.IsActive, cancellationToken);

        if (resource == null) return Result.Failure<Guid>("Resource not found or inactive");

        // Check availability with concurrency control
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

        // Calculate price
        var totalHours = (decimal)(request.EndDateTime - request.StartDateTime).TotalHours;
        decimal pricePerHour = 0;

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

        // Create booking (UserId = null for guest)
        var booking = new Booking
        {
            ResourceId = request.ResourceId,
            TenantId = request.TenantId,
            UserId = null, // Guest booking
            StartBookingDateTime = request.StartDateTime,
            EndBookingDateTime = request.EndDateTime.AddMinutes(-1),
            ReservationCode = reservationCode,
            TotalPrice = totalPrice,
            Status = BookingStatus.Pending, // Guest bookings start as Pending
            CustomerData = new CustomerData(
                request.CustomerName,
                request.CustomerEmail,
                request.CustomerPhone)
            {
                Name = request.CustomerName
            }
        };

        context.Bookings.Add(booking);
        await context.SaveChangesAsync(cancellationToken);

        // Add Outbox message for email/SMS sending
        if (!string.IsNullOrEmpty(request.CustomerEmail))
        {
            var outboxMessage = new OutboxMessage
            {
                Type = OutboxMessageType.BookingCreated,
                Content = JsonDocument.Parse(JsonSerializer.Serialize(new
                {
                    BookingId = booking.Id,
                    request.CustomerEmail,
                    request.CustomerName,
                    request.CustomerPhone,
                    request.ResourceId,
                    ReservationCode = reservationCode,
                    request.StartDateTime,
                    request.EndDateTime,
                    TotalPrice = totalPrice,
                    IsGuest = true
                })),
                Status = OutboxStatus.Pending,
                OccurredOn = DateTimeOffset.UtcNow
            };

            context.OutboxMessages.Add(outboxMessage);
            await context.SaveChangesAsync(cancellationToken);
        }

        logger.LogInformation("Guest booking created successfully with ID: {Id}, Code: {Code}",
            booking.Id, reservationCode);

        return Result.Success(booking.Id);
    }
}