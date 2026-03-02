using AgendaPlus.Domain.Common;
using MediatR;

namespace AgendaPlus.Application.Commands.Tenants;

public record DeleteTenantCommand(Guid Id) : IRequest<Result>;