using AgendaPlus.Domain.Common;
using MediatR;
using AvailabilityException = AgendaPlus.Domain.Entities.AvailabilityExceptions;

namespace AgendaPlus.Application.Queries.AvailabilityExceptions;

public record GetAvailabilityExceptionQuery(Guid Id) : IRequest<Result<AvailabilityException>>;