using AgendaPlus.Domain.Common;
using MediatR;

namespace AgendaPlus.Application.Commands.Bookings;

public record CreateBookingLoggedCommand(
    Guid ResourceId,
    DateTime StartDateTime,
    DateTime EndDateTime,
    string? IdempotencyKey = null) : IRequest<Result<Guid>>;