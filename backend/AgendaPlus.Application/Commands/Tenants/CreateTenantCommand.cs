using AgendaPlus.Domain.Common;
using MediatR;

namespace AgendaPlus.Application.Commands.Tenants;

public record CreateTenantCommand(
    string Name,
    string TimeZone,
    string Street,
    string City,
    string State,
    string ZipCode,
    string Country) : IRequest<Result<Guid>>;