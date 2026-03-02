using AgendaPlus.Domain.Common;
using MediatR;

namespace AgendaPlus.Application.Commands.Bookings;

public record CancelBookingCommand(Guid Id) : IRequest<Result>;