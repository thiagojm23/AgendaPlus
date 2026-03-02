using AgendaPlus.Application.DTOs.Responses;
using AgendaPlus.Application.Queries.Bookings;
using AgendaPlus.Application.QueryBuilders;
using AgendaPlus.Domain.Common;
using AgendaPlus.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AgendaPlus.Application.Handlers.Bookings;

public class CheckAvailabilityQueryHandler(
    ILogger<CheckAvailabilityQueryHandler> logger,
    ResourceQueryBuilder resourceQueryBuilder,
    BookingQueryBuilder bookingQueryBuilder,
    AvailabilityExceptionsQueryBuilder availabilityExceptionsQueryBuilder)
    : IRequestHandler<CheckAvailabilityQuery, Result<List<TimeSlotDto>>>
{
    private static readonly TimeSpan MinimumSlotDuration = TimeSpan.FromHours(1);

    public async Task<Result<List<TimeSlotDto>>> Handle(CheckAvailabilityQuery request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Checking availability for Resource {ResourceId} from {Start} to {End}",
            request.ResourceId, request.StartDateTime, request.EndDateTime);

        // Validate that the resource exists and is active
        var resource = await resourceQueryBuilder
            .AsNoTracking()
            .Query
            .FirstOrDefaultAsync(r => r.Id == request.ResourceId && r.IsActive, cancellationToken);

        if (resource == null)
        {
            logger.LogWarning("Resource not found or inactive: {ResourceId}", request.ResourceId);
            return Result.Failure<List<TimeSlotDto>>("Resource not found or inactive");
        }

        // Collect occupied intervals from bookings (Confirmed or Pending)
        var bookedIntervals = await bookingQueryBuilder
            .AsNoTracking()
            .Query
            .Where(b => b.ResourceId == request.ResourceId &&
                        (b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.Pending) &&
                        b.StartBookingDateTime < request.EndDateTime &&
                        b.EndBookingDateTime > request.StartDateTime)
            .Select(b => new { b.StartBookingDateTime, b.EndBookingDateTime })
            .ToListAsync(cancellationToken);

        // Collect occupied intervals from blocked availability exceptions
        var blockedIntervals = await availabilityExceptionsQueryBuilder
            .AsNoTracking()
            .Query
            .Where(e => e.ResourceId == request.ResourceId &&
                        e.Strategy == StrategyAvailabilityException.Block &&
                        e.StartBlockTime < request.EndDateTime &&
                        e.EndBlockTime > request.StartDateTime)
            .Select(e => new { Start = e.StartBlockTime, End = e.EndBlockTime })
            .ToListAsync(cancellationToken);

        // Build merged list of occupied intervals, clamped to the requested range
        var occupied = new List<(DateTime Start, DateTime End)>();

        foreach (var b in bookedIntervals)
        {
            var start = b.StartBookingDateTime < request.StartDateTime ? request.StartDateTime : b.StartBookingDateTime;
            var end = b.EndBookingDateTime > request.EndDateTime ? request.EndDateTime : b.EndBookingDateTime;
            occupied.Add((start, end));
        }

        foreach (var e in blockedIntervals)
        {
            var start = e.Start < request.StartDateTime ? request.StartDateTime : e.Start;
            var end = e.End > request.EndDateTime ? request.EndDateTime : e.End;
            occupied.Add((start, end));
        }

        // Sort and merge overlapping intervals
        var merged = MergeIntervals(occupied);

        // Find free slots as the complement of occupied within the requested range
        var freeSlots = new List<TimeSlotDto>();
        var cursor = request.StartDateTime;

        foreach (var (occStart, occEnd) in merged)
        {
            if (cursor < occStart && (occStart - cursor) >= MinimumSlotDuration)
                freeSlots.Add(new TimeSlotDto(cursor, occStart));
            cursor = occEnd > cursor ? occEnd : cursor;
        }

        // Add trailing free slot if any
        if (cursor < request.EndDateTime && (request.EndDateTime - cursor) >= MinimumSlotDuration)
            freeSlots.Add(new TimeSlotDto(cursor, request.EndDateTime));

        logger.LogInformation("Found {Count} available time slots for Resource {ResourceId}",
            freeSlots.Count, request.ResourceId);

        return Result.Success(freeSlots);
    }

    private static List<(DateTime Start, DateTime End)> MergeIntervals(
        List<(DateTime Start, DateTime End)> intervals)
    {
        if (intervals.Count == 0) return intervals;

        var sorted = intervals.OrderBy(i => i.Start).ToList();
        var merged = new List<(DateTime Start, DateTime End)> { sorted[0] };

        for (var i = 1; i < sorted.Count; i++)
        {
            var last = merged[^1];
            if (sorted[i].Start <= last.End)
                merged[^1] = (last.Start, sorted[i].End > last.End ? sorted[i].End : last.End);
            else
                merged.Add(sorted[i]);
        }

        return merged;
    }
}
