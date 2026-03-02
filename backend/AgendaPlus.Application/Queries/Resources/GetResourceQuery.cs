using AgendaPlus.Domain.Common;
using AgendaPlus.Domain.Entities;
using MediatR;

namespace AgendaPlus.Application.Queries.Resources;

public record GetResourceQuery(Guid Id) : IRequest<Result<Resource>>;