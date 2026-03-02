using AgendaPlus.Domain.Common;
using MediatR;

namespace AgendaPlus.Application.Commands.Bookings;

public record CreateBookingGuestCommand(
    Guid ResourceId,
    Guid TenantId,
    DateTime StartDateTime,
    DateTime EndDateTime,
    string CustomerName,
    string? CustomerEmail,
    string? CustomerPhone,
    string? IdempotencyKey = null) : IRequest<Result<Guid>>;