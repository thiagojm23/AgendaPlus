using AgendaPlus.Domain.Common;
using MediatR;
using AvailabilityException = AgendaPlus.Domain.Entities.AvailabilityExceptions;

namespace AgendaPlus.Application.Queries.AvailabilityExceptions;

public record ListAvailabilityExceptionsQuery(
    Guid? ResourceId = null,
    DateTime? StartDate = null,
    DateTime? EndDate = null,
    int PageNumber = 1,
    int PageSize = 10) : IRequest<Result<List<AvailabilityException>>>;