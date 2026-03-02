using AgendaPlus.Domain.Common;
using AgendaPlus.Domain.Entities;
using MediatR;

namespace AgendaPlus.Application.Queries.Bookings;

public record SearchBookingByEmailQuery(string Email) : IRequest<Result<List<Booking>>>;