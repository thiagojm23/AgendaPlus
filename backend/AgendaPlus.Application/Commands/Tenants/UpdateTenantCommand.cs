using AgendaPlus.Domain.Common;
using MediatR;

namespace AgendaPlus.Application.Commands.Tenants;

public record UpdateTenantCommand(
    Guid Id,
    string Name,
    string TimeZone,
    bool IsActive) : IRequest<Result>;