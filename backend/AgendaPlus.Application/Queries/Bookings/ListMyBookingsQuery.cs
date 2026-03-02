using AgendaPlus.Domain.Common;
using AgendaPlus.Domain.Entities;
using AgendaPlus.Domain.Enums;
using MediatR;

namespace AgendaPlus.Application.Queries.Bookings;

public record ListMyBookingsQuery(
    BookingStatus? Status = null,
    int PageNumber = 1,
    int PageSize = 10) : IRequest<Result<List<Booking>>>;