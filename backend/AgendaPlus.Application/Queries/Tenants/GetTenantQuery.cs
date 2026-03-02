using AgendaPlus.Domain.Common;
using AgendaPlus.Domain.Entities;
using MediatR;

namespace AgendaPlus.Application.Queries.Tenants;

public record GetTenantQuery(Guid Id) : IRequest<Result<Tenant>>;