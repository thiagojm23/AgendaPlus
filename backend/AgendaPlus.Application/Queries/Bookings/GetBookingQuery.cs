using AgendaPlus.Domain.Common;
using AgendaPlus.Domain.Entities;
using MediatR;

namespace AgendaPlus.Application.Queries.Bookings;

public record GetBookingQuery(Guid Id) : IRequest<Result<Booking>>;