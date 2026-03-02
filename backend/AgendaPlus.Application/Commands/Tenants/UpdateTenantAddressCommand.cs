using AgendaPlus.Domain.Common;
using MediatR;

namespace AgendaPlus.Application.Commands.Tenants;

public record UpdateTenantAddressCommand(
    Guid TenantId,
    string Street,
    string City,
    string State,
    string ZipCode,
    string Country) : IRequest<Result>;