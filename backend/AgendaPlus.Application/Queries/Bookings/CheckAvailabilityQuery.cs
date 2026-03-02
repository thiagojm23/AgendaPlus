using AgendaPlus.Application.DTOs.Responses;
using AgendaPlus.Domain.Common;
using MediatR;

namespace AgendaPlus.Application.Queries.Bookings;

public record CheckAvailabilityQuery(
    Guid ResourceId,
    DateTime StartDateTime,
    DateTime EndDateTime) : IRequest<Result<List<TimeSlotDto>>>;