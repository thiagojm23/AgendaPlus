using AgendaPlus.Domain.Common;
using AgendaPlus.Domain.Entities;
using AgendaPlus.Domain.Enums;
using MediatR;

namespace AgendaPlus.Application.Queries.Resources;

public record ListResourcesQuery(
    int PageNumber = 1,
    int PageSize = 10,
    bool? IsActive = null,
    ResourceType? Type = null) : IRequest<Result<List<Resource>>>;